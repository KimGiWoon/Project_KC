using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    public Node nodeData { get; private set; }

    // [추가!] 노드의 현재 상태를 나타내는 변수들
    public bool isRevealed { get; private set; } = false;
    public bool isSelectable { get; private set; } = false;

    private MapConfig _mapConfig;
    private SpriteRenderer _spriteRenderer;

    public void Setup(Node dataNode, MapConfig config)
    {
        this.nodeData = dataNode;
        this._mapConfig = config;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        isRevealed = false;
        isSelectable = false;

        // 시작 노드는 처음부터 공개되고 선택 가능한 상태입니다.
        if (nodeData.nodeType == NodeType.Start)
        {
            isRevealed = true;
            isSelectable = true;
        }

        UpdateVisuals(); // 상태에 맞는 첫 모습으로 설정
    }

    // [수정!] Reveal 함수는 이제 isRevealed 상태만 변경
    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;
    }

    // [추가!] 외부에서 이 노드의 선택 가능 여부를 제어하는 함수
    public void SetSelectable(bool selectable)
    {
        isSelectable = selectable;
        UpdateVisuals(); // 상태가 바뀌었으니 모습도 업데이트
    }

    // [추가!] 노드의 상태에 따라 색상과 모습을 결정하는 함수
    public void UpdateVisuals()
    {
        if (_spriteRenderer == null) return;

        // 아직 공개되지 않았다면 무조건 미스터리 아이콘
        if (!isRevealed)
        {
            ApplySprite(NodeType.Mystery);
            _spriteRenderer.color = Color.white; // 미스터리 아이콘은 원래 색상
            return;
        }

        // 공개되었다면 실제 타입에 맞는 스프라이트 적용
        ApplySprite(nodeData.nodeType);

        // 선택 불가능한 상태라면 어둡게 처리
        if (!isSelectable)
        {
            _spriteRenderer.color = Color.grey;
        }
        else // 선택 가능하다면 밝게 처리
        {
            _spriteRenderer.color = Color.white;
        }
    }

    // ApplySprite 함수는 이전과 동일 (내용은 아래에 유지)
    private void ApplySprite(NodeType type)
    {
        if (_mapConfig == null || _mapConfig.NodeTemplates == null) return;
        NodeTemplate template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == type);
        if (template == null) return;

        if (type == NodeType.Event)
        {
            List<Sprite> targetList = null;
            switch (nodeData.EventTypeKC)
            {
                case EventTypeKC.Positive: targetList = template.positiveEventSprites; break;
                case EventTypeKC.Negative: targetList = template.negativeEventSprites; break;
                case EventTypeKC.Neutral: targetList = template.neutralEventSprites; break;
            }
            if (targetList != null && targetList.Count > 0)
                _spriteRenderer.sprite = targetList[Random.Range(0, targetList.Count)];
        }
        else
        {
            if (template.sprite != null)
                _spriteRenderer.sprite = template.sprite;
        }

        if (type == NodeType.Start || type == NodeType.Boss)
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        else
            transform.localScale = Vector3.one;
    }
}