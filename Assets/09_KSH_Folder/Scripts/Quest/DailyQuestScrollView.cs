using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestScrollView : MonoBehaviour
{
    private ScrollRect _scrollRect;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();

        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 1f;
    }

    private void OnEnable()
    {
        if (_scrollRect.content != null)
        {
            var pos = _scrollRect.content.anchoredPosition;
            pos.y = 0f; // 최상단 위치
            _scrollRect.content.anchoredPosition = pos;
        }
    }
}