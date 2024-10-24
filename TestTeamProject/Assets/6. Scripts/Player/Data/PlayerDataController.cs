using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerDataController : NetworkBehaviour
{
    [SyncVar]
    public string nickname;

    [SyncVar(hook = nameof(OnIsDeadChanged))]
    public bool isDead = false;

    [SyncVar(hook = nameof(OnHpChanged))]
    public int hp = 1000;
    public int stamina = 1000;

    public GameObject hpBar;
    public GameObject staminaBar;
    public GameObject useTorch;
    public Animator animator;

    //횃불 토글 변수
    public bool isTorch = false;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        SetNickname(CustomNetworkRoomManager.Nickname != null && CustomNetworkRoomManager.Nickname != ""
            ? CustomNetworkRoomManager.Nickname : "No Nickname");
        hpBar = GameObject.Find("HPBar");
        staminaBar = GameObject.Find("StaminaBar");
        useTorch = GameObject.Find("useTorch");
        isTorch = GetComponent<PlayerMovementController>().isTorch;
    }

    [Command]
    public void SetNickname(string playerNickname)
    {
        nickname = playerNickname;
    }

    public void ChangeHp(int amount)
    {
        hp += amount;

        if (hp > 1000)
            hp = 1000;

        if (hp < 0)
        {
            hp = 0;
            isDead = true;
            animator.SetBool("isDead",true);
        }
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

        if (staminaBar != null)
            staminaBar.GetComponent<Image>().fillAmount = (float)stamina / 1000;
    }

    public void OnIsDeadChanged(bool oldDead, bool newDead)
    {
        if (!isLocalPlayer)
            return;

    }

    public void OnHpChanged(int oldHp, int newHp)
    {
        if (!isLocalPlayer)
            return;

        if (hpBar != null)
            hpBar.GetComponent<Image>().fillAmount = (float)newHp / 1000;
    }
    public void TorchIconControl()
    {
        isTorch = GetComponent<PlayerMovementController>().isTorch;
        if(isTorch)
        {
            useTorch.GetComponent<Image>().color = new Color(0,255,0,255);
            Debug.Log("torch icon color changed");
        }
        else 
        {
            useTorch.GetComponent<Image>().color = new Color(0, 255, 0, 0);
            Debug.Log("torch icon color changed");
        }
    }
}
