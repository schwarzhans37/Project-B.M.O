using System;
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

    public GameObject hpBar;
    public GameObject staminaBar;

    protected override void OnValidate()
    {
        base.OnValidate();
        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // 권한이 있으면 활성화
        this.enabled = true;
    }

    void Start()
    {
        if (!isLocalPlayer)
            return;

        hpBar = GameObject.Find("HPBar");
        staminaBar = GameObject.Find("StaminaBar");

        GameObject.Find("GameDataManager").GetComponent<GameDataController>().playerDataController = this;

        SetNickname(CustomNetworkRoomManager.Nickname != null && CustomNetworkRoomManager.Nickname != ""
            ? CustomNetworkRoomManager.Nickname : "No Nickname");
    }
    
    public void CmdReportTaskWorking()
    {
        GameDataController gameDataController = GameObject.Find("GameDataManager").GetComponent<GameDataController>();

        // 클라이언트의 연결 ID를 추가하여 작업 중임을 추적
        if (gameDataController.completedClients.Contains(connectionToClient.connectionId))
        {
            gameDataController.completedClients.Remove(connectionToClient.connectionId);
            Debug.Log($"Client {connectionToClient.connectionId} is working on the task.");
        }
    }

    [Command]
    public void CmdReportTaskComplete()
    {
        GameDataController gameDataController = GameObject.Find("GameDataManager").GetComponent<GameDataController>();

        // 클라이언트의 연결 ID를 추가하여 완료 여부를 추적
        if (!gameDataController.completedClients.Contains(connectionToClient.connectionId))
        {
            gameDataController.completedClients.Add(connectionToClient.connectionId);
            Debug.Log($"Client {connectionToClient.connectionId} has completed the task.");
        }
    }

    [Command]
    public void SetNickname(string playerNickname)
    {
        nickname = playerNickname;
    }

    [Command]
    public void CmdChangeHp(int amount)
    {
        ChangeHp(amount);
    }

    public void ChangeHp(int amount)
    {
        hp += amount;

        if (hp > 1000)
            hp = 1000;

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
        }
    }

    public void ChangeStamina(int amount)
    {
        stamina += amount;

        if (stamina >= 1000)
            stamina = 1000;

        if (stamina <= 0
            && amount < 0)
            stamina = -500;

        if (staminaBar != null)
            staminaBar.GetComponent<Image>().fillAmount = (float)stamina / 1000;
    }

    public void OnHpChanged(int oldHp, int newHp)
    {
        if (!isLocalPlayer)
            return;

        if (hpBar == null)
            return;
        
        StartCoroutine(PlayHpChangeEffect(oldHp, newHp));
    }

    IEnumerator PlayHpChangeEffect(float oldHp, float newHp)
    {
        float elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            if (newHp != hp)
                break;
            elapsedTime += Time.deltaTime * 2;
            hpBar.GetComponent<Image>().fillAmount = Mathf.Lerp(oldHp / 1000, newHp / 1000, elapsedTime);
            yield return null;
        }
        hpBar.GetComponent<Image>().fillAmount = (float)newHp / 1000;
    }

    public void OnIsDeadChanged(bool oldDead, bool newDead)
    {
        if (!isLocalPlayer)
            return;
            
        GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetPlayerUIActive(!newDead);
        GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetInteractiveUIActive(!newDead);
        StartCoroutine(GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetDeathedUIActive(newDead));

        if (newDead)
        {
            GetComponent<NetworkAnimator>().animator.SetBool("isDead",true);
            GetComponent<NetworkAnimator>().animator.SetTrigger("dead");
            GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetDeathCamActive(true, transform);
            GetComponent<PlayerCamera>().playerCamera.gameObject.SetActive(false);

            GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetTorchState(false);
            GetComponent<PlayerMovementController>().CmdTorch(false);

            GameObject.Find("GameDataManager").GetComponent<LightingManager>().TimeOfDay =
                GameObject.Find("GameDataManager").GetComponent<GameDataController>().time / 60f;
        }
        else
        {
            GetComponent<NetworkAnimator>().animator.SetBool("isDead", false);
            GetComponent<PlayerCamera>().playerCamera.gameObject.SetActive(true);
            GameObject.Find("PlayerManager").GetComponent<PlayerUIController>().SetDeathCamActive(false);
        }
    }
}
