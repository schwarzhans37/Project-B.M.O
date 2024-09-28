using Mirror;
using UnityEngine;


[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(GameDataView))]
public class GameDataController : NetworkBehaviour
{
    public GameDataView gameDataView;

    void Start()
    {
        gameDataView = GetComponent<GameDataView>();
    }

    [SyncVar(hook = nameof(OnMoneyChanged))]
    public int money; // 플레이어의 돈

    [SyncVar(hook = nameof(OnDayChanged))]
    public int day; // 게임 진행 일수

    [SyncVar(hook = nameof(OnHourChanged))]
    public int hour; // 게임 진행 시간

    [SyncVar(hook = nameof(OnMinuteChanged))]
    public int minute; // 게임 진행 분

    [SyncVar(hook = nameof(OnSecondChanged))]
    public int second; // 게임 진행 초

    [SyncVar(hook = nameof(OnIsDayChanged))]
    public bool isDay; // 낮인지 밤인지

    [SyncVar(hook = nameof(OnIsPausedChanged))]
    public bool isPaused; // 게임 일시정지 여부

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
    public void OnHourChanged(int oldHour, int newHour) { }
    public void OnMinuteChanged(int oldMinute, int newMinute) { }
    public void OnSecondChanged(int oldSecond, int newSecond) { }
    public void OnIsDayChanged(bool oldIsDay, bool newIsDay) { }
    public void OnIsPausedChanged(bool oldIsPaused, bool newIsPaused) { }
    
}
