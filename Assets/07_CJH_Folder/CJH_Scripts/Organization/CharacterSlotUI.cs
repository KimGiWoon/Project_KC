using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CJH
{
    public class CharacterSlotUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public CharacterData currentCharacterData;
        public UnityEvent<CharacterData> OnLongPress = new UnityEvent<CharacterData>();

        private Image characterImage;
        private const float requiredHoldTime = 1.0f;
        private bool isPointerDown = false;
        private float pointerDownTimer = 0f;
        private bool isLongPressTriggered = false;

        void Awake()
        {
            characterImage = GetComponent<Image>();
        }

        public void Setup(CharacterData data)
        {
            currentCharacterData = data;
            characterImage.sprite = data.characterIcon;

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

        void Update()
        {
            if (isPointerDown)
            {
                pointerDownTimer += Time.deltaTime;
                if (pointerDownTimer >= requiredHoldTime && !isLongPressTriggered)
                {
                    OnLongPress.Invoke(currentCharacterData);
                    isLongPressTriggered = true;
                    isPointerDown = false;
                }
            }
        }
    }
}