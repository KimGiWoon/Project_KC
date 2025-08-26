using System.Linq;
using UnityEngine;

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
        // 그 외(이벤트 노드)는 미스터리 상태로 시작
        else
        {
            // 미스테리 타입을 가진 NodeTemplate을 찾아서 스프라이트를 적용
            ApplySprite(NodeType.Mystery);
        }
    }

    // 노드의 실제 타입을 해당하는 스프라이트로 교체하여 공개
    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;

        // 이벤트 노드의 경우, 이제서야 실제 이벤트 타입(긍정/부정/중립)이 결정됩니다.
        // 현재는 이벤트 타입을 별도로 구분하지 않으므로, 그냥 Event 타입 스프라이트를 적용합니다.
        // 만약 긍정/부정/중립 아이콘이 다르다면 이 부분을 확장해야 합니다.
        ApplySprite(nodeData.nodeType);
    }

    // NodeType에 맞는 스프라이트를 찾아서 적용하는 함수
    private void ApplySprite(NodeType type)
    {
        if (_mapConfig == null || _mapConfig.NodeTemplates == null) return;

        // MapConfig에 있는 NodeTemplates 리스트에서 타입이 일치하는 첫 번째 템플릿을 찾습니다.
        NodeTemplate template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == type);

        if (template != null && template.sprite != null)
        {
            _spriteRenderer.sprite = template.sprite;
        }
        else
        {
            Debug.LogWarning($"{type} 타입에 해당하는 NodeTemplate 또는 스프라이트가 MapConfig에 없습니다!");
        }

        // 시작/보스 노드의 경우 특별히 크기를 키워줌
        if (type == NodeType.Start || type == NodeType.Boss)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one; // 그 외 노드는 기본 크기
        }
    }
}