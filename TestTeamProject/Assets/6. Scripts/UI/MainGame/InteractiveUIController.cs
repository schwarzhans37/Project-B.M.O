using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractiveUIController : MonoBehaviour
{
    public float rayDistance = 5f;  // Ray의 거리
    public CanvasGroup guideLine;   // 상호작용 가이드 UI
    public GameObject progressBar; // 원형 진행 바
    public float holdTime; // F키를 누르고 있어야 하는 시간
    private float holdProgress = 0f;

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
        if (GameUIController.IsPaused)
            return;

        // 카메라 중앙에서 Ray를 쏴서 오브젝트를 탐지
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Untagged"))
            {
                // UI 숨기기
                guideLine.alpha = 0;
                holdProgress = 0;
                UpdateProgressBar(0);
                return;
            }

            if (hit.collider.CompareTag("InteractiveObject"))
            {
                GameObject obj = hit.collider.gameObject;
                holdTime = obj.GetComponent<InteractionObject>().holdTime;

                // F키를 누르고 있으면 바를 채운다
                if (Input.GetKey(KeyCode.F))
                {
                    guideLine.alpha = 0;

                    if (holdTime == 0)
                    {
                        // 행동을 트리거
                        InteractWithObject(hit.collider.gameObject);
                        return;
                    }

                    holdProgress += Time.deltaTime;
                    UpdateProgressBar(holdProgress / holdTime);

                    if (holdProgress >= holdTime)
                    {
                        // 행동을 트리거
                        InteractWithObject(hit.collider.gameObject);
                        holdProgress = 0;
                    }
                }
                else
                {
                    // UI를 표시
                    guideLine.GetComponentInChildren<TMP_Text>().text = obj.GetComponent<InteractionObject>().guideText;
                    guideLine.alpha = 1;
                    holdProgress = 0;
                    UpdateProgressBar(0);
                }
            }
        }
        else
        {
            // Ray가 아무것도 맞추지 않으면 UI를 숨김
            guideLine.alpha = 0;
            holdProgress = 0;
            UpdateProgressBar(0);
        }
    }

    void InteractWithObject(GameObject obj)
    {
        // 상호작용 로직
        obj.GetComponent<InteractionObject>().InteractWithObject();
    }

    void UpdateProgressBar(float progress)
    {
        // 원형 진행 바 업데이트 로직
        progressBar.GetComponent<Image>().fillAmount = progress;
    }
}
