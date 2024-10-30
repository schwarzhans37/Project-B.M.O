using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Sydewa;
using UnityEngine;


[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(GameDataView))]
public class GameDataController : NetworkBehaviour
{
    public GameDataView gameDataView;

    public GameObject deathPoint;
    public GameObject spawnPoint;
    public GameObject sleepingPoint;
    public GameObject wagon;

    public GameObject forestSpawner;
    public GameObject dungeonSpawner;

    [SyncVar(hook = nameof(OnAllocatedAmountChanged))]
    public int allocatedAmount;

    [SyncVar(hook = nameof(OnMoneyChanged))]
    public int money; // 플레이어의 돈

    [SyncVar(hook = nameof(OnDayChanged))]
    public int day; // 게임 진행 일수

    [SyncVar(hook = nameof(OnTimeChanged))]
    public int time; // 게임 진행 시간

    public int startTime; // 게임 시작 시간
    public int maxGameTime; // 게임 진행 시간 상한

    [SyncVar(hook = nameof(OnIsDayChanged))]
    public bool isDay = true; // 낮 밤 여부

    public HashSet<int> completedClients = new HashSet<int>();
    public int totalClients; 
    public PlayerDataController playerDataController;

    public static bool IsinteractionLocked = true;
    public static bool IsMoveLocked = false;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameDataView = GetComponent<GameDataView>();
        startTime = (int)GetComponent<LightingManager>().StartTime;
        maxGameTime = 900 + startTime * 60;
    }

    IEnumerator Start()
    {
        if (!isServer)
            yield break;

        CustomNetworkRoomManager roomManager = NetworkManager.singleton as CustomNetworkRoomManager;
        while (completedClients.Count == roomManager.roomSlots.Count)
        {
            completedClients.Clear();
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.identity != null)
                    completedClients.Add(conn.connectionId);
            }
        }

        StartCoroutine(StartSetting());
    }

    public IEnumerator StartSetting()
    {
        if (!isServer)
            yield break;

        totalClients = NetworkServer.connections.Count;

        SetIsInteractionLocked(true);
        yield return new WaitForSeconds(1f);
        gameDataView.ShowGameView("게임 시작합니다.");
        yield return ConfirmClientsComplete(10f);
        gameDataView.ShowGameView("할당량을 확인하세요.");
        yield return ConfirmClientsComplete(10f);
        SetAllocatedAmount();
        yield return ConfirmClientsComplete(10f);
        SetIsInteractionLocked(false);
    }

    public IEnumerator ConfirmClientsComplete(float timeoutDuration)
    {
        totalClients = NetworkServer.connections.Count;
        if (completedClients.Count == totalClients)
            completedClients.Clear();

        float elapsedTime = 0f;
        while (elapsedTime < timeoutDuration)
        {
            elapsedTime += Time.deltaTime;
            if (completedClients.Count == totalClients)
                break;
            
            yield return null;
        }

        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;
            if (!completedClients.Contains(conn.connectionId))
                completedClients.Add(conn.connectionId);
        }
    }

    // 게임 시작 시 호출
    public IEnumerator StartGame(List<GameObject> players)
    {
        if (!isServer)
            yield break;

        forestSpawner.GetComponent<EnemySpawner>().SpawnEnemies((int)day/3);
        forestSpawner.GetComponent<SlenderManSpawner>().currentLevel = (int)day/3;
        dungeonSpawner.GetComponent<EnemySpawner>().SpawnEnemies((int)day/3);
        dungeonSpawner.GetComponent<ItemSpawner>().SpawnItems((int)day/3);

        SetIsInteractionLocked(false);
        SetIsMoveLocked(false);

        gameDataView.FadeOutBlackScreen();
        yield return ConfirmClientsComplete(10f);

        gameDataView.ShowTime();
        gameDataView.SetSunMovement(true);

        int deathedPlayersCount;
        float elapsedTime = 0f;
        while (isDay)
        {
            elapsedTime += Time.deltaTime;
            time = Mathf.RoundToInt(elapsedTime) + startTime * 60;

            if (time > 1140)
                forestSpawner.GetComponent<SlenderManSpawner>().SpawnSlenderMan();

            deathedPlayersCount = 0;
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity == null)
                    continue;
                if (conn.identity.GetComponent<PlayerDataController>().isDead)
                    deathedPlayersCount++;
            }

            if (deathedPlayersCount == NetworkServer.connections.Count
                || time > maxGameTime)
            {
                StartCoroutine(OnAllPlayersDead());
                break;
            }
            
            yield return null;
        }
    }

    // 게임 종료 시 호출
    public IEnumerator EndGame()
    {
        if (!isServer)
            yield break;

        isDay = false;
        gameDataView.HideTime();
        gameDataView.SetSunMovement(false);
        InitializeFields();

        SetIsInteractionLocked(false);
        SetIsMoveLocked(false);

        gameDataView.FadeOutBlackScreen();
        yield return ConfirmClientsComplete(10f);
    }

    // 모든 플레이어가 사망 시 호출
    public IEnumerator OnAllPlayersDead()
    {
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;
            conn.identity.GetComponent<PlayerDataController>().isDead = true;
        }
        
        SetIsInteractionLocked(true);
        SetIsMoveLocked(true);

        gameDataView.FadeOutBlackScreen();
        yield return ConfirmClientsComplete(10f);

        gameDataView.HideTime();
        gameDataView.SetSunMovement(false);
        InitializeFields();

        StartCoroutine(AdvanceDay());
    }

    // 하루가 지날 때 호출
    public IEnumerator AdvanceDay()
    {
        if (!isServer)
            yield break;

        day++;
        isDay = true;
        time = startTime * 60;

        gameDataView.SetSunTimePosition(startTime);
        yield return ConfirmClientsComplete(10f);

        totalClients = NetworkServer.connections.Count;
        List<string> dailyReportNickname = new List<string>();
        List<bool> dailyReportIsDead = new List<bool>();

        int deathedPlayersCount = 0;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;

            dailyReportNickname.Add(conn.identity.GetComponent<PlayerDataController>().nickname);
            if (conn.identity.GetComponent<PlayerDataController>().isDead)
            {
                conn.identity.GetComponent<InventoryController>().ClearItems();
                
                MoveToSpawnPoint(conn, conn.identity.gameObject, deathedPlayersCount);
                conn.identity.GetComponent<PlayerDataController>().CmdReportTaskWorking();
                yield return ConfirmClientsComplete(10f);

                dailyReportIsDead.Add(true);
                deathedPlayersCount++;
            }
            else
                dailyReportIsDead.Add(false);

            conn.identity.GetComponent<PlayerDataController>().isDead = false;
            conn.identity.GetComponent<PlayerDataController>().hp = 1000;
        }

        SetIsMoveLocked(false);

        gameDataView.FadeOutBlackScreen();
        yield return ConfirmClientsComplete(10f);

        gameDataView.ShowDailyReport(dailyReportNickname, dailyReportIsDead);
        yield return ConfirmClientsComplete(30f);

        money -= Mathf.RoundToInt(money * deathedPlayersCount / totalClients);

        yield return new WaitForSeconds(3f);

        if (day % 3 == 0)
        {
            if (allocatedAmount < money)
            {
                gameDataView.ShowGameView("생존 성공");
                yield return ConfirmClientsComplete(10f);
                money -= allocatedAmount;
                SetAllocatedAmount();
                yield return ConfirmClientsComplete(10f);
            }
            else
            {
                gameDataView.ShowGameView("할당량이 부족합니다.");
                yield return ConfirmClientsComplete(10f);
                gameDataView.ShowGameView("Game Over");
                yield return ConfirmClientsComplete(10f);
                gameDataView.FadeOutBlackScreen();
                yield return ConfirmClientsComplete(10f);
                yield return InitializeGame();
            }
        }

        SetIsInteractionLocked(false);
    }

    // 필드 초기화 시 호출
    public void InitializeFields()
    {
        forestSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        forestSpawner.GetComponent<SlenderManSpawner>().RemoveSlenderMan();
        forestSpawner.GetComponent<ItemSpawner>().RemoveItems();
        dungeonSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        dungeonSpawner.GetComponent<ItemSpawner>().RemoveItems();
    }

    // 게임 초기화 시 호출
    public IEnumerator InitializeGame()
    {
        if (!isServer)
            yield break;

        money = 0;
        day = 0;
        isDay = true;
        time = startTime * 60;

        List<GameObject> items = GameObject.FindGameObjectsWithTag("ItemObject").ToList();
        foreach (GameObject item in items)
            NetworkServer.Destroy(item);
        
        int index = 0;
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null)
                continue;
         
            conn.identity.GetComponent<PlayerDataController>().isDead = false;
            conn.identity.GetComponent<PlayerDataController>().hp = 1000;
            conn.identity.GetComponent<InventoryController>().ClearItems();

            MoveTodeathPoint(conn, conn.identity.gameObject, index);
            conn.identity.GetComponent<PlayerDataController>().CmdReportTaskWorking();
            yield return ConfirmClientsComplete(10f);

            index++;
        }
        
        gameDataView.FadeOutBlackScreen();
        yield return ConfirmClientsComplete(10f);
    }

    [TargetRpc]
    public void MoveToSpawnPoint(NetworkConnectionToClient target, GameObject player, int index)
    {
        player.transform.position = spawnPoint.transform.GetChild(index).position;
        player.transform.rotation = spawnPoint.transform.GetChild(index).rotation;
        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }

    [TargetRpc]
    public void MoveTodeathPoint(NetworkConnectionToClient target, GameObject player, int index)
    {
        player.transform.position = deathPoint.transform.GetChild(index).position;
        player.transform.rotation = deathPoint.transform.GetChild(index).rotation;
        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }

    public void SetAllocatedAmount()
    {
        totalClients = NetworkServer.connections.Count;
        allocatedAmount = (int)(Random.Range(150, 180) * totalClients * (1 + 0.2 * day / 3));
        gameDataView.ShowGameView("새로운 할당량:\n$" + allocatedAmount);
    }

    public void OnAllocatedAmountChanged(int oldAmount, int newAmount) { }

    public void OnMoneyChanged(int oldMoney, int newMoney)
    {
        StartCoroutine(gameDataView.OnMoneyChanged(oldMoney, newMoney));
    }

    public void OnDayChanged(int oldDay, int newDay) { }

    public void OnTimeChanged(int oldTime, int newTime)
    {
        gameDataView.OnTimeChanged(oldTime, newTime);
    }

    public void OnIsDayChanged(bool oldIsDay, bool newIsDay)
    {
        if (isServer)
        {
            sleepingPoint.GetComponent<SleepingPointContoller>().isInteractable = !newIsDay;
            wagon.GetComponent<WagonController>().isInteractable = newIsDay;
        }
    }

    [ClientRpc]
    public void SetIsInteractionLocked(bool isLocked)
    {
        IsinteractionLocked = isLocked;
    }

    [ClientRpc]
    public void SetIsMoveLocked(bool isLocked)
    {
        IsMoveLocked = isLocked;
    }
}
