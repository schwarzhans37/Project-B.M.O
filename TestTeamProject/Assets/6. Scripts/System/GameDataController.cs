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

    protected override void OnValidate()
    {
        base.OnValidate();
        gameDataView = GetComponent<GameDataView>();
        startTime = (int)GetComponent<LightingManager>().StartTime;

        maxGameTime = 900f;
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
        StartCoroutine(AdvanceDay(players, players.Count));
        
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

    public IEnumerator AdvanceDay(List<GameObject> deathPlayers, int palyerConunt)
    {
        if (!isServer)
            yield break;

        gameDataView.FadeOutBlackScreen();
        yield return new WaitForSeconds(1f);

        day++;
        isDay = true;
        time = startTime * 60;
        gameDataView.SetSunTimePosition(startTime);

        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform transform in spawnPoint.transform)
            spawnPoints.Add(transform);

        for (int i = 0; i < deathPlayers.Count; i++)
        {
            deathPlayers[i].GetComponent<PlayerDataController>().isDead = false;
            MoveToSpawnPoint(deathPlayers[i].GetComponent<NetworkIdentity>().connectionToClient, deathPlayers[i], spawnPoints[i]);
        }

        yield return new WaitForSeconds(3f);

        money -= Mathf.RoundToInt(money * deathPlayers.Count / palyerConunt);
    }

    [TargetRpc]
    public void MoveToSpawnPoint(NetworkConnectionToClient target, GameObject player, Transform spawnPoint)
    {
        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;
    }

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
