using System.Collections;
using Mirror;
using Sydewa;
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

    //횃불 토글 변수
    public bool isTorch = false;

    public bool isInteractionLock = false;
    public bool isMoveLock = false;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        SetNickname(CustomNetworkRoomManager.Nickname != null && CustomNetworkRoomManager.Nickname != ""
            ? CustomNetworkRoomManager.Nickname : "No Nickname");
        hpBar = GameObject.Find("HPBar");
        staminaBar = GameObject.Find("StaminaBar");
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
            
        GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetPlayerUIActive(!newDead);
        GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetInteractiveUIActive(!newDead);
        StartCoroutine(GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetDeathedUIActive(newDead));
        GameObject.Find("GameDataManager").GetComponent<LightingManager>().SunDirectionalLight.enabled = true;
        RenderSettings.skybox = GameObject.Find("GameDataManager").GetComponent<LightingManager>().skyboxMat;
        DynamicGI.UpdateEnvironment();

        StartCoroutine(ChangedDeadState(newDead));
    }

    public IEnumerator ChangedDeadState(bool newDead)
    {
        if (newDead)
        {
            GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetDeathCamActive(true, transform);
            GetComponent<PlayerCamera>().playerCamera.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            transform.position = Vector3.zero;
        }
        else
        {
            GetComponent<PlayerCamera>().playerCamera.gameObject.SetActive(true);
            GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetDeathCamActive(false);
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
