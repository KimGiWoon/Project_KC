using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KSH
{
    public class RelicUI : MonoBehaviour
    {
        [Header("유물 이미지")]
        [SerializeField] private Image relicImage; //유물 이미지
        [SerializeField] private Button relicButton; //유물 버튼
        [Header("유물 이름")]
        [SerializeField] private TextMeshProUGUI relicName;
        [Header("돋보기 버튼")]
        [SerializeField] private Button showButton;
        //[SerializeField] private Outline outline; //테두리

        private RelicDatas relic;
        private System.Action<RelicUI> onRelicClicked;

        public void SetData(RelicDatas relic, System.Action<RelicUI> clickCallBack, RelicDetailUI detailUI)
        {
            this.relic = relic;
            onRelicClicked = clickCallBack;

            relicImage.sprite = relic.relicImage;
            relicName.text = relic.relicName;

            relicButton.onClick.RemoveAllListeners();
            relicButton.onClick.AddListener(() => onRelicClicked?.Invoke(this));

            showButton.onClick.RemoveAllListeners();
            showButton.onClick.AddListener(() =>
            {
                detailUI.gameObject.SetActive(true);
                detailUI.ShowDetail(relic);
            });

            //outline.enabled = false;
        }

        //public void SetOutline(bool isOutline) => outline.enabled = isOutline;

        public RelicDatas GetRelic() => relic;
    }
}