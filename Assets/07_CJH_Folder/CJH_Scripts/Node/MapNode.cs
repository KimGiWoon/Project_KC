using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace CJH
{
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

        public void Setup(Node dataNode, MapConfig config)
        {
            nodeData = dataNode;
            _mapConfig = config;
            _spriteRenderer = GetComponent<SpriteRenderer>();

            isRevealed = false;
            isSelectable = false;

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
            if (selectable == false)
            {
                isSelectable = false;
                UpdateVisuals();
            }
            else StartCoroutine(DelayedSelectable());
        }

        private IEnumerator DelayedSelectable()
        {
            yield return new WaitForSeconds(1f);
            isSelectable = true;
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            if (_spriteRenderer == null) return;

            if (!isRevealed)
            {
                ApplySprite(NodeType.Mystery);
                _spriteRenderer.color = nonSelectableColor; // 선택 불가능한 색으로 표시
                return;
            }

            ApplySprite(nodeData.nodeType);

            if (MapView.Instance != null && MapView.Instance.CurrentMapData != null &&
                MapView.Instance.CurrentMapData.Path.Contains(nodeData))
                _spriteRenderer.color = visitedColor;
            else if (isSelectable)
                _spriteRenderer.color = selectableColor;
            else
                _spriteRenderer.color = nonSelectableColor;
        }

        private void ApplySprite(NodeType type)
        {
            if (_mapConfig == null || _mapConfig.NodeTemplates == null) return;

            // 이벤트 타입일 경우의 로직 변경
            if (type == NodeType.Event)
            {
                // 1. 현재 노드의 GroupID를 가져옵니다.
                int groupID = nodeData.GroupID;

                // 2. MapConfig에 만들어둔 함수를 이용해 GroupID에 맞는 아이콘을 찾습니다.
                var icon = _mapConfig.GetIconForEventGroup(groupID);

                // 3. 찾은 아이콘을 적용합니다.
                if (icon != null)
                {
                    _spriteRenderer.sprite = icon;
                }
                else
                {
                    // 혹시라도 해당하는 아이콘이 없으면 기본 이벤트 아이콘을 표시합니다.
                    var template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == type);
                    if (template != null) _spriteRenderer.sprite = template.sprite;
                }
            }
            // 이벤트가 아닌 다른 모든 노드 타입의 경우
            else
            {
                var template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == type);
                if (template != null && template.sprite != null)
                {
                    _spriteRenderer.sprite = template.sprite;
                }
            }

            if (type == NodeType.Start || type == NodeType.Boss)
                transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            else
                transform.localScale = Vector3.one;
        }

        public Sprite GetSpriteForNodeType()
        {
            if (_mapConfig == null) return null;

            if (nodeData.nodeType == NodeType.Event)
            {
                return _mapConfig.GetIconForEventGroup(nodeData.GroupID);
            }
            else
            {
                var template = _mapConfig.NodeTemplates.FirstOrDefault(t => t.nodeType == nodeData.nodeType);
                return template?.sprite;
            }
        }
    }
}