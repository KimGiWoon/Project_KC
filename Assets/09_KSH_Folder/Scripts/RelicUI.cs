using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace KSH
{
    public class RelicUI : MonoBehaviour
    {
        [SerializeField] private Image relicImage; //유물 이미지
        [SerializeField] private TextMeshProUGUI relicName; //유물 이름
        [SerializeField] private Button relicButton;
        [SerializeField] private Outline outline; //테두리
        
        private Relic relic;
        private System.Action<RelicUI> onRelicClicked;
        
        public void SetData(Relic relic, System.Action<RelicUI> clickCallBack)
        {
            this.relic = relic;
            onRelicClicked = clickCallBack;
            
            relicImage.sprite = relic.relicImage;
            relicName.text = relic.relicName;
            
            relicButton.onClick.RemoveAllListeners();
            relicButton.onClick.AddListener(() => onRelicClicked?.Invoke(this));
            
            outline.enabled = false;
        }
        
        public void SetOutline(bool isOutline) => outline.enabled = isOutline;
        
        public Relic GetRelic() => relic;
    }    
}
