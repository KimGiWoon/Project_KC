using UnityEngine;
using UnityEngine.EventSystems;

namespace CJH
{
    public class TeamSlotLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float requiredHoldTime = 1.0f; // 1초 이상 누르면 정보창 표시
        private float pointerDownTimer;
        private bool isPointerDown = false;

        public CharacterInfoPanel infoPanel; // 인스펙터에서 정보창 연결

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            pointerDownTimer = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        private void Update()
        {
            if (isPointerDown)
            {
                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer >= requiredHoldTime)
                {
                    // 타이머가 목표 시간에 도달하면 정보창을 띄움
                    if (infoPanel != null)
                    {
                        // infoPanel.Show(characterData); // 캐릭터 데이터와 함께 정보창 표시
                    }
                    Debug.Log("Long Press! Info panel should appear.");
                    isPointerDown = false; // 한 번만 실행되도록 초기화
                }
            }
        }
    }
}