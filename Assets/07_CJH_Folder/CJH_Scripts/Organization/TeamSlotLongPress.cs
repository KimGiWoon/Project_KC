using UnityEngine;
using UnityEngine.EventSystems;

namespace CJH
{
    public class TeamSlotLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private const float LONG_PRESS_TIME = 1.0f; // 1초
        private float pointerDownTimer = 0f;
        private bool isPointerDown = false;
        private bool isLongPressTriggered = false;

        private CharacterData characterData;
        private CharacterInfoPanel infoPanel;

        /// <summary>
        /// 외부(TeamManager)에서 호출하여 슬롯의 데이터를 설정합니다.
        /// </summary>
        public void Setup(CharacterData data, CharacterInfoPanel panel)
        {
            this.characterData = data;
            this.infoPanel = panel;
        }

        private void Update()
        {
            if (isPointerDown && !isLongPressTriggered)
            {
                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer >= LONG_PRESS_TIME)
                {
                    // 1초가 지났으면 캐릭터 세부정보 실행
                    isLongPressTriggered = true;
                    if (characterData != null && infoPanel != null)
                    {
                        infoPanel.ShowPanel(characterData);
                        Debug.Log($"{characterData.characterName} 정보 창 열기");
                    }
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            isLongPressTriggered = false;
            pointerDownTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;
        }
    }
}