using UnityEngine;
using UnityEngine.EventSystems; // UI 이벤트 처리를 위해 필수

public class UIBackgroundDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    // 인스펙터에서 기존에 카메라에 있던 CameraDragControl 스크립트를 연결해줍니다.
    [Tooltip("카메라에 붙어있는 CameraDragControl 스크립트를 여기에 연결하세요.")]
    public CameraDragControl cameraControl;

    private Vector3 lastMousePosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시점의 마우스 위치를 저장합니다.
        // 이렇게 하면 기존 CameraDragControl과 동일한 방식으로 델타 값을 계산할 수 있습니다.
        if (cameraControl != null)
        {
            lastMousePosition = Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중일 때 CameraDragControl 스크립트의 카메라 이동 로직을 호출합니다.
        if (cameraControl != null)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            cameraControl.MoveCamera(delta); // 수정된 함수 호출
            lastMousePosition = Input.mousePosition;
        }
    }
}