using System.Collections;
using Mirror;
using Sydewa;
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

    public GameObject playerUI;
    public GameObject interactiveUI;
    public GameObject deathedUI;
    public GameObject hpBar;
    public GameObject staminaBar;
    public GameObject deathCam;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        SetNickname(CustomNetworkRoomManager.Nickname != null && CustomNetworkRoomManager.Nickname != ""
            ? CustomNetworkRoomManager.Nickname : "No Nickname");
        playerUI = GameObject.Find("PlayerUI");
        interactiveUI = GameObject.Find("InteractiveUI");
        deathedUI = GameObject.Find("DeatedhUI");
        hpBar = GameObject.Find("HPBar");
        staminaBar = GameObject.Find("StaminaBar");
        deathCam = GameObject.Find("DeathStateCamera");
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

        playerUI.SetActive(!newDead);
        interactiveUI.SetActive(!newDead);
        deathedUI.SetActive(newDead);
        GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled = true;

        StartCoroutine(ChangedDeadState(newDead));
    }

    public IEnumerator ChangedDeadState(bool newDead)
    {
        if (newDead)
        {
            deathCam.SetActive(true);
            GetComponent<PlayerCamera>().playerCamera.gameObject.SetActive(false);
        }
        else
        {
            GetComponent<PlayerCamera>().playerCamera.gameObject.SetActive(true);
            deathCam.SetActive(false);
        }

        yield break;
    }

    public void OnHpChanged(int oldHp, int newHp)
    {
        if (!isLocalPlayer)
            return;

        if (hpBar != null)
            hpBar.GetComponent<Image>().fillAmount = (float)newHp / 1000;
    }
}
