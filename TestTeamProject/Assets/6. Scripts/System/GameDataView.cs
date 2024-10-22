using System.Collections;
using Mirror;
using Sydewa;
using TMPro;
using UnityEngine;

public class GameDataView : NetworkBehaviour
{    
    public CanvasGroup moneyView;
    public CanvasGroup timeView;
    public CanvasGroup dlackScreen;

    public float changeDuration = 1f; // 변화에 걸리는 시간 (초)
    public float displayDuration = 3f; // 변화 후 표시되는 시간 (초)
    public float fadeDuration = 1f; // 사라지는 시간 (초)


    public IEnumerator OnMoneyChanged(int oldMoney, int newMoney)
    {
        // 텍스트 및 알파 초기 설정
        moneyView.alpha = 1;
        TMP_Text moneyText = moneyView.GetComponentInChildren<TMP_Text>();

        float elapsedTime = 0f;
        
        // 1초 동안 돈을 점진적으로 변경
        while (elapsedTime < changeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / changeDuration;
            int currentMoney = Mathf.RoundToInt(Mathf.Lerp(oldMoney, newMoney, t));
            moneyText.text = currentMoney.ToString() + " $";
            yield return null;
        }
        
        // 최종 값 설정
        moneyText.text = newMoney.ToString() + " $";

        // 3초 동안 유지
        yield return new WaitForSeconds(displayDuration);

        // 서서히 사라지기 (알파 값 줄이기)
        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            moneyView.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        // 완전히 투명하게 설정
        moneyView.alpha = 0;
    }

    public void OnTimeChanged(int oldTime, int newTime)
    {
        int hour = newTime / 60;
        int minute = newTime % 60;
        timeView.GetComponentInChildren<TMP_Text>().text = string.Format("{0:D2} : {1:D2}", hour, minute);
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
    }

    [ClientRpc]
    public void FadeOutBlackScreen()
    {
        StartCoroutine(ApplyBlackScreenEffect());
    }

    public IEnumerator ApplyBlackScreenEffect()
    {
        // alpha 값을 0에서 1로 증가
        dlackScreen.alpha = 0f;
        while (dlackScreen.alpha < 1)
        {
            dlackScreen.alpha += Time.deltaTime * (2.5f - dlackScreen.alpha * 2);
            yield return null;
        }

        // 완전히 불투명하게 설정
        dlackScreen.alpha = 1;

        // duration 동안 대기
        float elapsed = 0;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        // alpha 값을 1에서 0으로 감소
        while (dlackScreen.alpha > 0)
        {
            elapsed += Time.deltaTime;
            dlackScreen.alpha -= Time.deltaTime * (0.5f + dlackScreen.alpha);
            yield return null;
        }

        // 완전히 투명하게 설정
        dlackScreen.alpha = 0;
    }
}
