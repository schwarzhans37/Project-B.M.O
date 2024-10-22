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
    public IEnumerator StartGame()
    {
        if (!isServer)
            yield break;

        forestSpawner.GetComponent<EnemySpawner>().SpawnEnemies((int)day/3);
        forestSpawner.GetComponent<SlenderManSpawner>().currentLevel = (int)day/3;
        dungeonSpawner.GetComponent<EnemySpawner>().SpawnEnemies((int)day/3);
        dungeonSpawner.GetComponent<ItemSpawner>().SpawnItems((int)day/3);

        gameDataView.FadeOutBlackScreen();
        gameDataView.ShowTime();
        yield return new WaitForSeconds(3f);

        gameDataView.SetSunMovement(true);

        float elapsedTime = 0f;
        while (isDay)
        {
            elapsedTime += Time.deltaTime;
            time = Mathf.RoundToInt(elapsedTime) + startTime * 60;

            if (time > 1140 || true)
                forestSpawner.GetComponent<SlenderManSpawner>().SpawnSlenderMan();

            yield return null;
        }
    }

    // 게임 종료 시 호출
    public IEnumerator EndGame()
    {
        if (!isServer)
            yield break;

        isDay = false;
        forestSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        forestSpawner.GetComponent<SlenderManSpawner>().RemoveSlenderMan();
        dungeonSpawner.GetComponent<EnemySpawner>().RemoveEnemies();
        dungeonSpawner.GetComponent<ItemSpawner>().RemoveItems();

        gameDataView.FadeOutBlackScreen();
        gameDataView.HideTime();
        gameDataView.SetSunMovement(false);

        yield return new WaitForSeconds(3f);

        forestSpawner.GetComponent<FieldPlayerKiller>().KillPlayer();
        dungeonSpawner.GetComponent<FieldPlayerKiller>().KillPlayer();
    }

    public IEnumerator OnAllPlayersDead()
    {
        List<GameObject> players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerDataController>().isDead = true;
        }

        StartCoroutine(AdvanceDay(players, 4));
        yield return null;
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
        
        foreach (GameObject deathPlayer in deathPlayers)
        {
            deathPlayer.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            deathPlayer.GetComponent<PlayerDataController>().isDead = false;
        }

        yield return new WaitForSeconds(3f);

        money -= Mathf.RoundToInt(money * deathPlayers.Count / palyerConunt);
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
