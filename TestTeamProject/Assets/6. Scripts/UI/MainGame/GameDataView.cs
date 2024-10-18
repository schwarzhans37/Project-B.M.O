using System.Collections;
using TMPro;
using UnityEngine;

public class GameDataView : MonoBehaviour
{
    public CanvasGroup singleMoneyView;
    public float changeDuration = 1f; // 변화에 걸리는 시간 (초)
    public float displayDuration = 3f; // 변화 후 표시되는 시간 (초)
    public float fadeDuration = 1f; // 사라지는 시간 (초)

    public IEnumerator OnMoneyChanged(int oldMoney, int newMoney)
    {
        // 텍스트 및 알파 초기 설정
        singleMoneyView.alpha = 1;
        TMP_Text moneyText = singleMoneyView.GetComponentInChildren<TMP_Text>();

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

        // 4초 동안 유지
        yield return new WaitForSeconds(displayDuration);

        // 서서히 사라지기 (알파 값 줄이기)
        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            singleMoneyView.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        // 완전히 투명하게 설정
        singleMoneyView.alpha = 0;
    }
}
