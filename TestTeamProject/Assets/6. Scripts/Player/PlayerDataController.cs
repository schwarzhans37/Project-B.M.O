using Mirror;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerDataController : NetworkBehaviour
{
    
    [SyncVar(hook = nameof(OnHpChanged))]
    public int hp = 1000;
    [SyncVar(hook = nameof(OnStaminaChanged))]
    public int stamina = 1000;

    public GameObject hpBar;
    public GameObject staminaBar;

    void Start()
    {
        hpBar = GameObject.Find("HPBar");
        staminaBar = GameObject.Find("StaminaBar");
    }

    [Command]
    public void AddHp(int amount)
    {
        hp += hp < 1000 ? amount : 1000 - hp;
    }

    [Command]
    public void SubtractHp(int amount)
    {
        hp -= hp > 0 ? amount : hp;
    }

    [Command]
    public void AddStamina()
    {
        stamina += stamina < 1000 ? (int)(Time.deltaTime * 1000 / 5) : 1000 - stamina;
    }

    [Command]
    public void SubtractStamina()
    {
        stamina -= stamina > 0 ? (int)(Time.deltaTime * 1000 / 5) : stamina + 500;
    }

    public void OnHpChanged(int oldHp, int newHp)
    {
        if (!isLocalPlayer)
            return;

        hpBar.GetComponent<Image>().fillAmount = (float)newHp / 1000;
    }

    public void OnStaminaChanged(int oldStamina, int newStamina)
    {
        if (!isLocalPlayer)
            return;

        staminaBar.GetComponent<Image>().fillAmount = (float)newStamina / 1000;
    }

}
