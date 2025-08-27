using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    public Node nodeData { get; private set; }
    public bool isRevealed { get; private set; } = false;

    // MapConfig는 MapView를 통해 받아옵니다.
    private MapConfig _mapConfig;
    private SpriteRenderer _spriteRenderer;

    // 맵 생성 시, 노드의 데이터를 설정하고 초기 상태의 스프라이트를 적용
    public void Setup(Node dataNode, MapConfig config)
    {
        this.nodeData = dataNode;
        this._mapConfig = config;

        //  SpriteRenderer 컴포넌트를 연결
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("MapNode 오브젝트에 SpriteRenderer가 없습니다!", gameObject);
            return;
        }

        isRevealed = false;

        // 시작, 전투, 보스 노드는 처음부터 바로 공개
        if (nodeData.nodeType == NodeType.Start || nodeData.nodeType == NodeType.Battle || nodeData.nodeType == NodeType.Boss)
        {
            Reveal();
        }
        // 이벤트 노드는 미스터리 상태로 시작
        else
        {
            //NodeTemplate을 찾아서 스프라이트를 적용
            ApplyMysterySprite();
        }
    }

    // 노드의 실제 타입을 해당하는 스프라이트로 교체하여 공개
    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;

        // 이벤트 노드의 경우, 이벤트 타입(긍정/부정/중립)이 결정됩니다..
        ApplySprite(nodeData.nodeType);
    }

    // NodeType에 맞는 스프라이트를 찾아서 적용하는 함수
    private void ApplySprite(NodeType type)
    {
        if (_mapConfig == null || _mapConfig.NodeTemplates == null) return;

        NodeTemplate template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == type);

        if (template == null)
        {
            Debug.LogWarning($"{type} 타입에 해당하는 NodeTemplate이 MapConfig에 없습니다!");
            return;
        }

        // 이벤트 타입일 경우, 세부 타입에 따라 랜덤 스프라이트를 선택
        if (type == NodeType.Event)
        {
            List<Sprite> targetList = null;
            switch (nodeData.EventTypeKC)
            {
                case EventTypeKC.Positive:
                    targetList = template.positiveEventSprites;
                    break;
                case EventTypeKC.Negative:
                    targetList = template.negativeEventSprites;
                    break;
                case EventTypeKC.Neutral:
                    targetList = template.neutralEventSprites;
                    break;
            }

            if (targetList != null && targetList.Count > 0)
            {
                // 리스트에서 랜덤으로 스프라이트 하나를 선택하여 적용
                _spriteRenderer.sprite = targetList[Random.Range(0, targetList.Count)];
            }
            else
            {
                Debug.LogWarning($"{nodeData.EventTypeKC} 이벤트에 해당하는 스프라이트가 EventNodeTemplate에 등록되지 않았습니다.");
            }
        }
        else // 이벤트가 아닌 다른 모든 노드 타입의 경우
        {
            if (template.sprite != null)
            {
                _spriteRenderer.sprite = template.sprite;
            }
            else
            {
                Debug.LogWarning($"{type} 타입의 NodeTemplate에 기본 스프라이트가 없습니다!");
            }
        }

        // 크기 조절 로직
        if (type == NodeType.Start || type == NodeType.Boss)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void ApplyMysterySprite()
    {
        if (_mapConfig == null || _mapConfig.NodeTemplates == null) return;

        NodeTemplate template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == NodeType.Event);
        if (template == null || template.sprite == null)
        {
            Debug.LogWarning("이벤트용 미스터리 스프라이트가 없습니다!");
            return;
        }

        _spriteRenderer.sprite = template.sprite;
        transform.localScale = Vector3.one;
    }
}