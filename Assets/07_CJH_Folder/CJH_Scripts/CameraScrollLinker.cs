using UnityEngine;
// using UnityEngine.UI; // ScrollRect�� ����ϱ� ���� �ʿ��մϴ�.
using UnityEngine.UI;

public class CameraScrollLinker : MonoBehaviour
{
    [Header("������ UI �� ī�޶�")]
    public ScrollRect targetScrollRect;
    public Transform cameraTransform;

    [Header("ī�޶� �̵� ����")]
    public float cameraMinY = 0f;
    public float cameraMaxY = 10f;

    void Start()
    {
        // [����] ScrollRect�� ���� ����� ������ �̺�Ʈ�� �޵��� �����մϴ�.
        if (targetScrollRect != null)
        {
            targetScrollRect.onValueChanged.AddListener(UpdateCameraPosition);

            // �� ���� �� ���� ��ũ�� ��ġ�� ���� ī�޶� ����ȭ�մϴ�.
            // ScrollRect�� verticalNormalizedPosition�� ���� 1, �Ʒ��� 0�Դϴ�.
            UpdateCameraPosition(new Vector2(0, targetScrollRect.verticalNormalizedPosition));
        }
    }

    // [����] ScrollRect�� onValueChanged �̺�Ʈ�� Vector2 ���� �����ϹǷ�,
    // �Ű����� Ÿ���� Vector2�� �����մϴ�.
    private void UpdateCameraPosition(Vector2 scrollPosition)
    {
        if (cameraTransform == null) return;

        // Y�� ��ũ�� ���� �ʿ��ϹǷ� scrollPosition.y�� ����մϴ�.
        float scrollValue = scrollPosition.y;

        // Mathf.Lerp�� ����� ������ ������ �����մϴ�.
        float newYPosition = Mathf.Lerp(cameraMinY, cameraMaxY, scrollValue);

        Vector3 newCameraPosition = cameraTransform.position;
        newCameraPosition.y = newYPosition;
        cameraTransform.position = newCameraPosition;
    }

    private void OnDestroy()
    {
        // [����] ����ߴ� �����ʸ� ScrollRect���� �����մϴ�.
        if (targetScrollRect != null)
        {
            targetScrollRect.onValueChanged.RemoveListener(UpdateCameraPosition);
        }
    }
}