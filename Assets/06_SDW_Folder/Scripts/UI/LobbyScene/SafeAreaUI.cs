using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaUI : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private RectTransform[] _topView;
    [SerializeField] private RectTransform[] _bottomView;
    [SerializeField] private RectTransform[] _centerView;
    [SerializeField] private RectTransform[] _fullScreenView;

    [Header("Safe Area Settings")]
    public bool _applyToTopView = true;
    public bool _applyToBottomView = true;
    public bool _applyToCenterView = true;
    public bool _applyToFullScreenView = false;

    [Header("Padding (Optional)")]
    public float _topPadding = 10f;
    public float _bottomPadding = 10f;
    public float _sidePadding = 20f;

    private Rect _safeArea;
    private Vector2 _minAnchor;
    private Vector2 _maxAnchor;

    /// <summary>
    /// 시작 시 Safe Area를 반영하여 UI 요소들의 레이아웃을 업데이트
    /// </summary>
    private void Start() => ApplySafeArea();

    /// <summary>
    /// Safe Area가 변할 경우, ApplySafeArea를 다시 적용
    /// </summary>
    private void Update()
    {
        if (_safeArea != Screen.safeArea) ApplySafeArea();
    }

    /// <summary>
    /// Safe Area 변경 사항을 반영하여 UI 요소들의 레이아웃을 업데이트
    /// </summary>
    private void ApplySafeArea()
    {
        _safeArea = Screen.safeArea;

        _minAnchor = _safeArea.position;
        _maxAnchor = _safeArea.position + _safeArea.size;

        _minAnchor.x /= Screen.width;
        _minAnchor.y /= Screen.height;
        _maxAnchor.x /= Screen.width;
        _maxAnchor.y /= Screen.height;

        if (_topView != null && _applyToTopView) ApplyToTopView();
        if (_bottomView != null && _applyToBottomView) ApplyToBottomView();
        if (_centerView != null && _applyToCenterView) ApplyToCenterView();
        if (_fullScreenView != null && _applyToFullScreenView) ApplyToFullScreenView();
    }

    /// <summary>
    /// Safe Area 변경 사항을 반영하여 지정된 Top View UI 요소들의 레이아웃을 업데이트
    /// </summary>
    private void ApplyToTopView()
    {
        foreach (var top in _topView)
        {
            //# 상단 View - Safe Area의 상단부터 화면 중앙(50%)까지
            top.anchorMin = new Vector2(_minAnchor.x, 0.5f);
            top.anchorMax = new Vector2(_maxAnchor.x, _maxAnchor.y);

            //# 패딩 적용
            top.offsetMin = new Vector2(_sidePadding, 0);
            top.offsetMax = new Vector2(-_sidePadding, -_topPadding);
        }
    }

    /// <summary>
    /// Safe Area 변경 사항을 반영하여 지정된 Bottom View UI 요소들의 레이아웃을 업데이트
    /// </summary>
    private void ApplyToBottomView()
    {
        foreach (var bottom in _bottomView)
        {
            //# 하단 View - Safe Area의 하단부터 화면 중앙(50%)까지
            bottom.anchorMin = new Vector2(_minAnchor.x, _minAnchor.y);
            bottom.anchorMax = new Vector2(_maxAnchor.x, 0.5f);

            //# 패딩 적용
            bottom.offsetMin = new Vector2(_sidePadding, _bottomPadding);
            bottom.offsetMax = new Vector2(-_sidePadding, 0);
        }
    }

    /// <summary>
    /// Safe Area 변경 사항을 반영하여 Center View UI 요소들의 레이아웃을 업데이트
    /// </summary>
    private void ApplyToCenterView()
    {
        foreach (var center in _centerView)
        {
            //# 중앙 View - Safe Area 전체 영역 사용
            center.anchorMin = _minAnchor;
            center.anchorMax = _maxAnchor;

            //# 패딩 적용
            center.offsetMin = new Vector2(_sidePadding, _bottomPadding);
            center.offsetMax = new Vector2(-_sidePadding, -_topPadding);
        }
    }

    /// <summary>
    /// Safe Area 변경 사항을 반영하여 Full Screen UI 요소들의 레이아웃을 업데이트
    /// </summary>
    private void ApplyToFullScreenView()
    {
        foreach (var full in _fullScreenView)
        {
            //# 전체 화면 View - Safe Area 무시하고 전체 화면 사용
            full.anchorMin = Vector2.zero;
            full.anchorMax = Vector2.one;
            full.offsetMin = Vector2.zero;
            full.offsetMax = Vector2.zero;
        }
    }
}