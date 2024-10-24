using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class InteractionController : NetworkBehaviour
{
    public float rayDistance = 4f;  // Ray의 거리
    public CanvasGroup guideLine;   // 상호작용 가이드 UI
    public GameObject progressBar; // 원형 진행 바
    public float holdTime; // F키를 누르고 있어야 하는 시간
    public float waitTime; // 다음 상호작용을 위한 대기 시간 
    private float holdProgress = 0f;
    private float lastWaitTime;

    protected override void OnValidate()
    {
        base.OnValidate();

        waitTime = 0.5f;
        lastWaitTime = -waitTime;
        this.enabled = false;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        // 권한이 있으면 상호작용 컨트롤러를 활성화
        this.enabled = true;
    }

    void Start()
    {
        if (guideLine == null)
        {
            guideLine = GameObject.Find("GuideLine").GetComponent<CanvasGroup>();
            guideLine.alpha = 0;
        }

        if (progressBar == null)
        {
            progressBar = GameObject.Find("ProgressBar");
            UpdateProgressBar(0);
        }
    }

    void Update()
    {
        if (!isLocalPlayer
            || GetComponent<PlayerDataController>().isDead)
            return;

        if (GameUIController.IsPaused
            || GameDataController.isinteractionLocked
            || Time.time - lastWaitTime < waitTime)
        {
            HideUI();
            return;
        }

        // 카메라 중앙에서 Ray를 쏴서 오브젝트를 탐지
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            HideUI();
            return;
        }

        GameObject obj = hit.collider.gameObject;

        if (hit.collider.CompareTag("ItemObject"))
        {
            ShowguideLine("줍기 : [우클릭]");
            if (Input.GetMouseButtonDown(1))
            {
                GetComponent<InventoryController>().PickUpItem(obj);
            }
        }
        else if (hit.collider.CompareTag("InteractableObject"))
        {
            InteractableObject interactable = obj.GetComponent<InteractableObject>();
            holdTime = interactable.holdTime;

            if (!interactable.isInteractable)
            {
                HideUI();
                return;
            }

            if (Input.GetKey(KeyCode.E))
            {
                HandleTriggerStay(obj);
            }
            else
            {
                ShowguideLine(interactable.guideText);
            }
        }
        else
        {
            HideUI();
        }
    }

    void HandleTriggerStay(GameObject gameObject)
    {
        guideLine.alpha = 0;

        if (holdTime == 0)
        {
            // 행동을 트리거
            InteractWithObject(gameObject, this.gameObject);
            lastWaitTime = Time.time;
            return;
        }

        holdProgress += Time.deltaTime;
        UpdateProgressBar(holdProgress / holdTime);

        if (holdProgress >= holdTime)
        {
            // 행동을 트리거
            InteractWithObject(gameObject, this.gameObject);
            lastWaitTime = Time.time;
        }
    }

    void ShowguideLine(string text)
    {
        guideLine.GetComponentInChildren<TMP_Text>().text = text;
        guideLine.alpha = 1;
        holdProgress = 0;
        UpdateProgressBar(0);
    }

    void HideUI()
    {
        guideLine.alpha = 0;
        holdProgress = 0;
        UpdateProgressBar(0);
    }

    void UpdateProgressBar(float progress)
    {
        // 원형 진행 바 업데이트 로직
        progressBar.GetComponent<Image>().fillAmount = progress;
    }

    [Command]
    void InteractWithObject(GameObject obj, GameObject player)
    {
        // 상호작용 로직
        StartCoroutine(obj.GetComponent<InteractableObject>().InteractWithObject(player));
    }
}
