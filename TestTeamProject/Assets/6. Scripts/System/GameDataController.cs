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
    private float maxGameTime; // 게임 진행 시간 상한

    [SyncVar(hook = nameof(OnIsDayChanged))]
    public bool isDay = true; // 낮 밤 여부

    [SyncVar(hook = nameof(OnIsPausedChanged))]
    public bool isPaused = false; // 게임 일시정지 여부

    public static bool isinteractionLocked = true;
    public static bool isMoveLocked = false;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameDataView = GetComponent<GameDataView>();
        startTime = (int)GetComponent<LightingManager>().StartTime;

        maxGameTime = 900f;
    }

    IEnumerator Start()
    {
        if (!isServer)
            yield break;

        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        List<NetworkRoomPlayer> roomPlayers = GameObject.FindObjectsOfType<NetworkRoomPlayer>().ToList();
        while (players.Count != roomPlayers.Count)
        {
            Debug.Log(players.Count);
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        gameDataView.ShowGameView("게임 시작합니다.");
        yield return new WaitForSeconds(3.2f);
        gameDataView.ShowGameView("할당량을 확인하세요.");
        yield return new WaitForSeconds(3.2f);
        SetAllocatedAmount();
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

        gameDataView.FadeOutBlackScreen();
        yield return new WaitForSeconds(2f);

        gameDataView.ShowTime();
        gameDataView.SetSunMovement(true);

        List<GameObject> SurvivedPlayers = players;
        float elapsedTime = 0f;
        while (isDay)
        {
            elapsedTime += Time.deltaTime;
            time = Mathf.RoundToInt(elapsedTime) + startTime * 60;

            if (time > 1140 || true)
                forestSpawner.GetComponent<SlenderManSpawner>().SpawnSlenderMan();

            foreach (GameObject player in SurvivedPlayers)
            {
                if (player == null)
                {
                    SurvivedPlayers.Remove(player);
                    break;
                }
                if (player.GetComponent<PlayerDataController>().isDead)
                {
                    SurvivedPlayers.Remove(player);
                    break;
                }
            }

            if (SurvivedPlayers.Count == 0)
            {
                StartCoroutine(OnAllPlayersDead());
                break;
            }
            
            yield return null;
        }
    }

    // 게임 종료 시 호출
    public IEnumerator EndGame(List<GameObject> SurvivedPlayers, List<GameObject> players)
    {
        if (!isServer)
            yield break;

        isDay = false;

        gameDataView.FadeOutBlackScreen();

        foreach (GameObject player in players)
        {
            if (player == null)
                continue;

            if (!SurvivedPlayers.Contains(player))
                player.GetComponent<PlayerDataController>().isDead = true;
        }

        yield return new WaitForSeconds(1f);
        gameDataView.HideTime();
        gameDataView.SetSunMovement(false);
        InitializeFields();
    }


    public IEnumerator OnAllPlayersDead()
    {

        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        StartCoroutine(AdvanceDay(players));
        
        yield return new WaitForSeconds(1f);
        gameDataView.HideTime();
        gameDataView.SetSunMovement(false);
        InitializeFields();
    }

    public void InitializeFields()
    {
        forestSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        forestSpawner.GetComponent<SlenderManSpawner>().RemoveSlenderMan();
        forestSpawner.GetComponent<ItemSpawner>().RemoveItems();
        dungeonSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        dungeonSpawner.GetComponent<ItemSpawner>().RemoveItems();
    }

    public IEnumerator AdvanceDay(List<GameObject> players)
    {
        if (!isServer)
            yield break;

        gameDataView.FadeOutBlackScreen();
        yield return new WaitForSeconds(1f);

        day++;
        isDay = true;
        time = startTime * 60;
        gameDataView.SetSunTimePosition(startTime);

        List<string> dailyReportNickname = new List<string>();
        List<bool> dailyReportIsDead = new List<bool>();
        int deathedPlayersCount = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
                i--;
                continue;
            }

            dailyReportNickname.Add(players[i].GetComponent<PlayerDataController>().nickname);
            dailyReportIsDead.Add(players[i].GetComponent<PlayerDataController>().isDead);

            if (!players[i].GetComponent<PlayerDataController>().isDead)
                continue;

            deathedPlayersCount++;
            players[i].GetComponent<PlayerDataController>().isDead = false;
            players[i].GetComponent<PlayerDataController>().hp = 1000;
            players[i].GetComponent<InventoryController>().ClearItems();
            MoveToSpawnPoint(players[i].GetComponent<NetworkIdentity>().connectionToClient, players[i], spawnPoint.transform.GetChild(i).position);
        }

        yield return new WaitForSeconds(1f);

        gameDataView.ShowDailyReport(dailyReportNickname, dailyReportIsDead);

        yield return new WaitForSeconds(6f + dailyReportIsDead.Count * 0.5f);

        money -= Mathf.RoundToInt(money * deathedPlayersCount / players.Count);

        yield return new WaitForSeconds(3f);

        if (day % 3 != 0)
            yield break;

        if (allocatedAmount < money)
        {
            gameDataView.ShowGameView("생존 성공");
            yield return new WaitForSeconds(3.02f);
            money -= allocatedAmount;
            SetAllocatedAmount();
        }
        else
        {
            gameDataView.ShowGameView("할당량이 부족합니다.");
            yield return new WaitForSeconds(3.2f);
            gameDataView.ShowGameView("Game Over");
            yield return new WaitForSeconds(1f);
            gameDataView.FadeOutBlackScreen();
            yield return new WaitForSeconds(1f);
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            foreach (GameObject player in players)
                MoveToSpawnPoint(player.GetComponent<NetworkIdentity>().connectionToClient, player, transform.position);
        }
    }

    public IEnumerator InitializeGame()
    {
        if (!isServer)
            yield break;

        gameDataView.FadeOutBlackScreen();
        yield return new WaitForSeconds(1f);

        day = 0;
        time = startTime * 60;
        money = 0;
        isDay = true;

        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null)
            {
                players.RemoveAt(i);
                i--;
                continue;
            }

            players[i].GetComponent<PlayerDataController>().isDead = false;
            players[i].GetComponent<PlayerDataController>().hp = 1000;
            players[i].GetComponent<InventoryController>().ClearItems();
            MoveToSpawnPoint(players[i].GetComponent<NetworkIdentity>().connectionToClient, players[i], spawnPoint.transform.GetChild(i).position);
        }

        List<GameObject> items = GameObject.FindGameObjectsWithTag("ItemObject").ToList();
        foreach (GameObject item in items)
            NetworkServer.Destroy(item);

        yield return new WaitForSeconds(3f);

        gameDataView.ShowGameView("게임 시작합니다.");
        yield return new WaitForSeconds(3.2f);
        gameDataView.ShowGameView("할당량을 확인하세요.");
        yield return new WaitForSeconds(3.2f);
        SetAllocatedAmount();
    }

    [TargetRpc]
    public void MoveToSpawnPoint(NetworkConnectionToClient target, GameObject player, Vector3 spawnPoint)
    {
        player.transform.position = spawnPoint;
        player.transform.rotation = Quaternion.identity;
    }

    public void SetAllocatedAmount()
    {
        List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
        allocatedAmount = (int)(Random.Range(150, 180) * players.Count * (1 + 0.2 * day / 3));
        gameDataView.ShowGameView("새로운 할당량:\n$" + allocatedAmount);
    }

    public void OnAllocatedAmountChanged(int oldAmount, int newAmount) { }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void SubtractMoney(int amount)
    {
        money -= amount;
    }

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
    public void OnIsPausedChanged(bool oldIsPaused, bool newIsPaused) { }
    
}
