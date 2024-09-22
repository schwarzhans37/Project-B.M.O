using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractiveUIController : MonoBehaviour
{
    public float rayDistance = 5f;  // Ray의 거리
    public CanvasGroup guideText;   // 상호작용 가이드 UI
    public GameObject progressBar; // 원형 진행 바
    public float holdTime; // F키를 누르고 있어야 하는 시간
    private float holdProgress = 0f;

    void Start()
    {
        if (guideText == null)
        {
            guideText = GameObject.Find("GuideText").GetComponent<CanvasGroup>();
            guideText.alpha = 0;
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
            if (hit.collider.CompareTag("InnerDoor"))
            {
                holdTime = 0.5f;

                // F키를 누르고 있으면 바를 채운다
                if (Input.GetKey(KeyCode.F))
                {
                    guideText.alpha = 0;
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
                    guideText.GetComponentInChildren<TMP_Text>().text = "문 열기 [F]";
                    guideText.alpha = 1;
                    holdProgress = 0;
                    UpdateProgressBar(0);
                }
            }
            else if (hit.collider.CompareTag("OutsideDoor"))
            {
                holdTime = 1f;

                if (Input.GetKey(KeyCode.F))
                {
                    guideText.alpha = 0;
                    holdProgress += Time.deltaTime;
                    UpdateProgressBar(holdProgress / holdTime);

                    if (holdProgress >= holdTime)
                    {
                        InteractWithObject(hit.collider.gameObject);
                        holdProgress = 0;
                    }
                }
                else
                {
                    guideText.GetComponentInChildren<TMP_Text>().text = "들어가기 [F]";
                    guideText.alpha = 1;
                    holdProgress = 0;
                    UpdateProgressBar(0);
                }
            }
            else
            {
                // UI 숨기기
                guideText.alpha = 0;
                holdProgress = 0;
                UpdateProgressBar(0);
            }
        }
        else
        {
            // Ray가 아무것도 맞추지 않으면 UI를 숨김
            guideText.alpha = 0;
            holdProgress = 0;
            UpdateProgressBar(0);
        }
    }

    void InteractWithObject(GameObject obj)
    {
        // 상호작용 로직
        Debug.Log("Interacted with " + obj.name);
    }

    void UpdateProgressBar(float progress)
    {
        // 원형 진행 바 업데이트 로직
        progressBar.GetComponent<Image>().fillAmount = progress;
    }
}
