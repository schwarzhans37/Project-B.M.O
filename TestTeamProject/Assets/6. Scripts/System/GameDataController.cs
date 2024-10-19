using System.Collections;
using System.Collections.Generic;
using Mirror;
using Sydewa;
using UnityEngine;


[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(GameDataView))]
public class GameDataController : NetworkBehaviour
{
    public GameDataView gameDataView;
    public LightingManager lightingManager;

    public GameObject spawnPoint;
    public GameObject sleepingPoint;
    public GameObject wagon;

    public GameObject forestSpawner;
    public GameObject dungeonSpawner;

    [SyncVar(hook = nameof(OnMoneyChanged))]
    public int money = 0; // 플레이어의 돈

    [SyncVar(hook = nameof(OnDayChanged))]
    public int day = 0; // 게임 진행 일수

    [SyncVar(hook = nameof(OnTimeChanged))]
    public int time = 0; // 게임 진행 시간
    private int startTime; // 게임 시작 시간
    private float maxGameTime; // 게임 진행 시간 상한

    [SyncVar(hook = nameof(OnIsDayChanged))]
    public bool isDay = true; // 낮 밤 여부

    [SyncVar(hook = nameof(OnIsPausedChanged))]
    public bool isPaused = false; // 게임 일시정지 여부

    protected override void OnValidate()
    {
        base.OnValidate();
        gameDataView = GetComponent<GameDataView>();
        lightingManager = GetComponent<LightingManager>();
        startTime = (int)lightingManager.StartTime;

        maxGameTime = 900f;
    }

    // 게임 시작 시 호출
    public IEnumerator StartGame()
    {
        gameDataView.ShowTime();
        lightingManager.IsDayCycleOn = true;

        forestSpawner.GetComponent<EnemySpawner>().SpawnEnemies((int)day/3);
        dungeonSpawner.GetComponent<EnemySpawner>().SpawnEnemies((int)day/3);

        float elapsedTime = 0f;
        while (elapsedTime < maxGameTime && isDay)
        {
            elapsedTime += Time.deltaTime;
            time = Mathf.RoundToInt(elapsedTime) + startTime * 60;
            yield return null;
        }
    }

    // 게임 종료 시 호출
    public IEnumerator EndGame()
    {
        gameDataView.HideTime();
        lightingManager.IsDayCycleOn = false;
        isDay = false;

        forestSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        dungeonSpawner.GetComponent<EnemySpawner>().RemoveEnemies();

        yield return null;
    }

    public IEnumerator OnAllPlayersDead()
    {
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerDataController>().isDead = true;
        }

        yield return null;
        
        AdvanceDay(players);
    }

    public void AdvanceDay(List<GameObject> deathPlayers)
    {
        day++;
        isDay = true;
        lightingManager.TimeOfDay = startTime;

        List<Transform> spawnPoints = new List<Transform>();
        foreach (Transform transform in spawnPoint.transform)
            spawnPoints.Add(transform);
        
        foreach (GameObject deathPlayer in deathPlayers)
        {
            deathPlayer.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            deathPlayer.GetComponent<PlayerDataController>().isDead = false;
        }
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
        gameDataView.OnMoneyChanged(oldMoney, newMoney);
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
