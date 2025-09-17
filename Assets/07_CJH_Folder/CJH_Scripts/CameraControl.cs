using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragControl : MonoBehaviour
{
    [Header("카메라 이동 범위")]
    public float minY = 0f;
    public float maxY = 12f;

    [Header("드래그 민감도")]
    public float dragSpeed = 0.01f;

    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private bool isManualDragEnabled = true;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // 수동 드래그가 비활성화 상태이면 아무것도 하지 않음
        if (!isManualDragEnabled) return;

        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 위에서 클릭한 것이 아니라면 드래그 시작
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
        }

        // 마우스 버튼에서 손을 뗐을 때
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // 드래그 중일 때
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // Y축으로만 이동
            float newY = transform.position.y - delta.y * dragSpeed;

            // 카메라를 정해진 범위 안에 있도록 위치를 제한
            float clampedY = Mathf.Clamp(newY, minY, maxY);

            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);

            lastMousePosition = Input.mousePosition;
        }
    }

    // MapView에서 자동 이동 시 수동 조작을 막기 위한 함수
    public void SetManualDrag(bool isEnabled)
    {
        isManualDragEnabled = isEnabled;
        // 외부에서 드래그를 비활성화할 때, 현재 드래그 상태도 강제로 중지
        if (!isEnabled)
        {
            isDragging = false;
        }
    }

    public void MoveCamera(Vector3 mouseDelta)
    {
        // Y축으로만 이동
        float newY = transform.position.y - mouseDelta.y * dragSpeed;

        // 카메라를 정해진 범위 안에 있도록 위치를 제한
        float clampedY = Mathf.Clamp(newY, minY, maxY);

        transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
    }
}