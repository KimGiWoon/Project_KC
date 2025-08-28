using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    public Node nodeData { get; private set; }
    public bool isRevealed { get; private set; } = false;
    public bool isSelectable { get; private set; } = false;

    [Header("노드 상태별 색상")]
    public Color visitedColor = new Color(0.8f, 0.7f, 0.5f);
    public Color selectableColor = Color.white;
    public Color nonSelectableColor = Color.grey;

    private MapConfig _mapConfig;
    private SpriteRenderer _spriteRenderer;

    // 한 번 결정된 이벤트 스프라이트를 저장할 변수
    private Sprite chosenEventSprite;

    public void Setup(Node dataNode, MapConfig config)
    {
        this.nodeData = dataNode;
        this._mapConfig = config;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        isRevealed = false;
        isSelectable = false;
        chosenEventSprite = null; // 맵을 새로 만들 때마다 초기화

        if (nodeData.nodeType == NodeType.Start || nodeData.nodeType == NodeType.Battle || nodeData.nodeType == NodeType.Boss)
        {
            isRevealed = true;
            if (nodeData.nodeType == NodeType.Start)
            {
                isSelectable = true;
            }
        }
        UpdateVisuals();
    }

    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;
    }

    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (_spriteRenderer == null) return;

        if (!isRevealed)
        {
            ApplySprite(NodeType.Mystery);
            _spriteRenderer.color = selectableColor;
            return;
        }

        ApplySprite(nodeData.nodeType);

        if (MapView.Instance != null && MapView.Instance.CurrentMapData != null && MapView.Instance.CurrentMapData.Path.Contains(nodeData))
            _spriteRenderer.color = visitedColor;
        else if (isSelectable)
            _spriteRenderer.color = selectableColor;
        else
            _spriteRenderer.color = nonSelectableColor;
    }

    private void ApplySprite(NodeType type)
    {
        if (_mapConfig == null || _mapConfig.NodeTemplates == null) return;
        NodeTemplate template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == type);
        if (template == null) return;

        // 이벤트 타입일 경우의 로직 변경
        if (type == NodeType.Event)
        {
            // 아직 결정된 스프라이트가 없다면, 지금 랜덤으로 하나를 뽑아서 저장합니다.
            if (chosenEventSprite == null)
            {
                List<Sprite> targetList = null;
                switch (nodeData.EventTypeKC)
                {
                    case EventTypeKC.Positive: targetList = template.positiveEventSprites; break;
                    case EventTypeKC.Negative: targetList = template.negativeEventSprites; break;
                    case EventTypeKC.Neutral: targetList = template.neutralEventSprites; break;
                }
                if (targetList != null && targetList.Count > 0)
                {
                    chosenEventSprite = targetList[Random.Range(0, targetList.Count)];
                }
            }

            // 이미 결정된 스프라이트가 있다면, 그것을 계속 사용합니다.
            if (chosenEventSprite != null)
            {
                _spriteRenderer.sprite = chosenEventSprite;
            }
        }
        else // 이벤트가 아닌 다른 모든 노드 타입의 경우
        {
            if (template.sprite != null)
            {
                _spriteRenderer.sprite = template.sprite;
            }
        }

        if (type == NodeType.Start || type == NodeType.Boss)
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        else
            transform.localScale = Vector3.one;
    }
}