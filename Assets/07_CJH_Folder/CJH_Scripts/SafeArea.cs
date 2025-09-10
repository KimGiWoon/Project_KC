using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform panel;
    private Rect currentSafeArea = new Rect(0, 0, 0, 0);
    private ScreenOrientation currentOrientation = ScreenOrientation.AutoRotation;

    void Awake()
    {
        panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if (currentSafeArea != Screen.safeArea || currentOrientation != Screen.orientation)
            ApplySafeArea();
    }

    void ApplySafeArea()
    {
        currentSafeArea = Screen.safeArea;
        currentOrientation = Screen.orientation;

        // Safe area 좌표를 0~1 비율로 변환
        Vector2 anchorMin = currentSafeArea.position;
        Vector2 anchorMax = currentSafeArea.position + currentSafeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;

        Debug.Log("[SafeArea] Applied: " + currentSafeArea);
    }
}