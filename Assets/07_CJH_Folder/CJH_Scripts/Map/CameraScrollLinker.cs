using UnityEngine;
using UnityEngine.UI;

public class CameraScrollLinker : MonoBehaviour
{
    [Header("연결할 UI 및 카메라")]
    public ScrollRect targetScrollRect;
    public Transform cameraTransform;

    [Header("카메라 이동 범위")]
    public float cameraMinY = 0f;
    public float cameraMaxY = 10f;

    void Start()
    {
        //ScrollRect의 값이 변경될 때마다 이벤트를 받도록 수정합니다.
        if (targetScrollRect != null)
        {
            targetScrollRect.onValueChanged.AddListener(UpdateCameraPosition);

            // 씬 시작 시 현재 스크롤 위치에 맞춰 카메라를 동기화합니다.
            UpdateCameraPosition(new Vector2(0, targetScrollRect.verticalNormalizedPosition));
        }
    }


    private void UpdateCameraPosition(Vector2 scrollPosition)
    {
        if (cameraTransform == null) return;

        // Y축 스크롤 값이 필요하므로 scrollPosition.y를 사용합니다.
        float scrollValue = scrollPosition.y;

        // Mathf.Lerp를 사용한 나머지 로직은 동일합니다.
        float newYPosition = Mathf.Lerp(cameraMinY, cameraMaxY, scrollValue);

        Vector3 newCameraPosition = cameraTransform.position;
        newCameraPosition.y = newYPosition;
        cameraTransform.position = newCameraPosition;
    }

    private void OnDestroy()
    {
        if (targetScrollRect != null)
        {
            targetScrollRect.onValueChanged.RemoveListener(UpdateCameraPosition);
        }
    }
}