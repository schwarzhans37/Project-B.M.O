using UnityEngine;

public class HideLocalPlayer : MonoBehaviour
{
    void Start()
    {
        // 자신의 카메라에서만 로컬 플레이어 레이어를 제외함
        Camera mainCamera = GetComponentInChildren<Camera>();
        if (mainCamera != null)
        {
            // LocalPlayer 레이어를 제외하고 렌더링하도록 Culling Mask 설정
            mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LocalPlayer"));
        }
    }
}
