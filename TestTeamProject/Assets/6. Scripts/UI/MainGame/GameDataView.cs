using TMPro;
using UnityEngine;

public class GameDataView : MonoBehaviour
{
    public CanvasGroup singleMoneyView;

    public void OnMoneyChanged(int oldMoney, int newMoney)
    {
        singleMoneyView.alpha = 1;
        singleMoneyView.GetComponentInChildren<TMP_Text>().text = newMoney.ToString() + " $";
    }
}
