using Mirror;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerDataController : NetworkBehaviour
{

    [SyncVar(hook = nameof(OnDeadChanged))]
    public bool isDead = false;

    [SyncVar(hook = nameof(OnHpChanged))]
    public int hp = 1000;
    public int stamina = 1000;

    public GameObject hpBar;
    public GameObject staminaBar;

    void Start()
    {
        hpBar = GameObject.Find("HPBar");
        staminaBar = GameObject.Find("StaminaBar");
    }

    public void ChangeHp(int amount)
    {
        hp += amount;

        if (hp > 1000)
            hp = 1000;

        if (hp < 0)
            hp = 0;
    }

    public void AdjustStaminaOverTime(float time)
    {
        if (time == 0)
            return;

        stamina += (int)(Time.deltaTime * 1000 / time);

        if (stamina > 1000)
            stamina = 1000;

        if (stamina < 0 && time < 0)
            stamina = -300;

        staminaBar.GetComponent<Image>().fillAmount = (float)stamina / 1000;
    }

    public void OnDeadChanged(bool oldDead, bool newDead)
    {
        if (!isLocalPlayer)
            return;

    }

    public void OnHpChanged(int oldHp, int newHp)
    {
        if (!isLocalPlayer)
            return;

        hpBar.GetComponent<Image>().fillAmount = (float)newHp / 1000;
    }
}
