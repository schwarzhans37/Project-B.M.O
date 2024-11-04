using System.Collections;
using System.Collections.Generic;
using Mirror;
using Sydewa;
using TMPro;
using UnityEngine;

public class GameDataView : NetworkBehaviour
{ 
    public GameObject DailyReportPanel;
    public CanvasGroup gameView;
    public CanvasGroup moneyView;
    public CanvasGroup timeView;
    public CanvasGroup dlackScreen;

    public IEnumerator OnMoneyChanged(int oldMoney, int newMoney)
    {        
        // 텍스트 및 알파 초기 설정
        moneyView.alpha = 1;
        TMP_Text moneyText = moneyView.GetComponentInChildren<TMP_Text>();

        // 1초 동안 돈을 점진적으로 변경
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            int currentMoney = Mathf.RoundToInt(Mathf.Lerp(oldMoney, newMoney, elapsedTime));
            moneyText.text =  "$" + currentMoney.ToString();
            yield return null;
        }
        
        // 최종 값 설정
        moneyText.text = "$" + newMoney.ToString();

        // 3초 동안 유지
        yield return new WaitForSeconds(3f);

        // 서서히 사라지기 (알파 값 줄이기)
        while (moneyView.alpha > 0f)
        {
            moneyView.alpha -= Time.deltaTime;
            yield return null;
        }
        moneyView.alpha = 0;
    }

    public void OnTimeChanged(int oldTime, int newTime)
    {
        int hour = newTime / 60;
        int minute = newTime % 60;
        timeView.GetComponentInChildren<TMP_Text>().text = string.Format("{0:D2} : {1:D2}", hour, minute);
    }

    [ClientRpc]
    public void ShowDailyReport(List<string> dailyReportNickname, List<bool> dailyReportIsDead)
    {
        DailyReportPanel.transform.GetChild(0).GetComponent<TMP_Text>().text 
            = $"[ D-{(GetComponent<GameDataController>().day % 3 == 0 ? "Day" : 3 - GetComponent<GameDataController>().day % 3)} ]\n\n${GetComponent<GameDataController>().allocatedAmount}";

        int deathedPlayersCount = 0;
        for (int i = 0; i < dailyReportNickname.Count; i++)
        {
            DailyReportPanel.transform.GetChild(1).GetChild(i).GetChild(0)
            .GetComponent<TMP_Text>().text = dailyReportNickname[i];
            DailyReportPanel.transform.GetChild(1).GetChild(i).GetChild(1)
            .GetComponent<TMP_Text>().text = dailyReportIsDead[i] ? "[ 사망 ]" : "[ 생존 ]";
            DailyReportPanel.transform.GetChild(1).GetChild(i).GetChild(1)
            .GetComponent<CanvasGroup>().alpha = 0;
            if (dailyReportIsDead[i])
                deathedPlayersCount++;
        }

        DailyReportPanel.transform.GetChild(2).GetComponent<CanvasGroup>().alpha = 0;

        StartCoroutine(ApplyDailyReportEffect(dailyReportIsDead, deathedPlayersCount));
    }

    public IEnumerator ApplyDailyReportEffect(List<bool> dailyReportIsDead, int deathedPlayersCount)
    {
        while (DailyReportPanel.GetComponent<CanvasGroup>().alpha < 1)
            DailyReportPanel.GetComponent<CanvasGroup>().alpha += Time.deltaTime;
        DailyReportPanel.GetComponent<CanvasGroup>().alpha = 1;

        for (int i = 0; i < dailyReportIsDead.Count; i++)
        {
            yield return new WaitForSeconds(1f);
            DailyReportPanel.transform.GetChild(1).GetChild(i).GetChild(1)
            .GetComponent<CanvasGroup>().alpha = 1;
        }
        yield return new WaitForSeconds(1f);

        DailyReportPanel.transform.GetChild(2).GetComponent<CanvasGroup>().alpha = 1;
        int decrement = Mathf.RoundToInt(GetComponent<GameDataController>().money * deathedPlayersCount / dailyReportIsDead.Count);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            int currentMoney = Mathf.RoundToInt(Mathf.Lerp(0f, decrement, elapsedTime));
            DailyReportPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = "- $" + currentMoney.ToString();
            yield return null;
        }
        yield return new WaitForSeconds(3f);

        while (DailyReportPanel.GetComponent<CanvasGroup>().alpha > 0)
            DailyReportPanel.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
        DailyReportPanel.GetComponent<CanvasGroup>().alpha = 0;

        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }

    [ClientRpc]
    public void ShowGameView(string message)
    {
        StartCoroutine(ApplyGameViewEffect(message));
    }

    public IEnumerator ApplyGameViewEffect(string message)
    {
        // alpha 값을 0에서 1로 증가
        gameView.GetComponentInChildren<TMP_Text>().text = message;
        gameView.alpha = 0f;
        while (gameView.alpha < 1)
        {
            gameView.alpha += Time.deltaTime * (2.5f - gameView.alpha * 2);
            yield return null;
        }
        gameView.alpha = 1;

        yield return new WaitForSeconds(1f);

        // alpha 값을 1에서 0으로 감소
        while (gameView.alpha > 0)
        {
            gameView.alpha -= Time.deltaTime;
            yield return null;
        }
        gameView.alpha = 0;
        gameView.GetComponentInChildren<TMP_Text>().text = "";

        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }

    [ClientRpc]
    public void ShowTime()
    {
        timeView.alpha = 1;
    }

    [ClientRpc]
    public void HideTime()
    {
        timeView.alpha = 0;
    }

    [ClientRpc]
    public void SetSunMovement(bool isDay)
    {
        GetComponent<LightingManager>().IsDayCycleOn = isDay;
    }

    [ClientRpc]
    public void SetSunTimePosition(int time)
    {
        GetComponent<LightingManager>().TimeOfDay = time;
        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }

    [ClientRpc]
    public void FadeOutBlackScreen()
    {
        StartCoroutine(ApplyBlackScreenEffect());
    }

    public IEnumerator ApplyBlackScreenEffect()
    {
        if (dlackScreen.alpha == 0)
        {
            // alpha 값을 0에서 1로 증가
            dlackScreen.alpha = 0f;
            while (dlackScreen.alpha < 1)
            {
                dlackScreen.alpha += Time.deltaTime * (2.5f - dlackScreen.alpha * 2);
                yield return null;
            }
            dlackScreen.alpha = 1;
        }
        else
        {
            // alpha 값을 1에서 0으로 감소
            while (dlackScreen.alpha > 0)
            {
                dlackScreen.alpha -= Time.deltaTime * (0.5f + dlackScreen.alpha);
                yield return null;
            }
            dlackScreen.alpha = 0;
        }
        NetworkClient.localPlayer.GetComponent<PlayerDataController>().CmdReportTaskComplete();
    }
}
