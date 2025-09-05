using System.Collections;
using System.Collections.Generic;
using KSH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicDetailUI : MonoBehaviour
{
    [SerializeField] private Image relicDetailImage;
    [SerializeField] private TextMeshProUGUI relicDetailName;
    [SerializeField] private TextMeshProUGUI relicDetailDescription1;
    [SerializeField] private TextMeshProUGUI relicDetailDescription2;
    [SerializeField] private GameObject DetailUI;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(BackButtonClicked);
    }

    public void ShowDetail(RelicDatas relic)
    {
        relicDetailImage.sprite = relic.relicImage;
        relicDetailName.text = relic.relicName;
        relicDetailDescription1.text = relic.relicDescription.Count > 0 ? relic.relicDescription[0] : "";
        relicDetailDescription2.text = relic.relicDescription.Count > 1 ? relic.relicDescription[1] : "";
        DetailUI.SetActive(true);
    }
    
    public void BackButtonClicked()
    {
        DetailUI.SetActive(false);
    }

}
