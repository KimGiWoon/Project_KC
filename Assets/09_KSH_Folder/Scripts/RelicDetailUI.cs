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

    public void ShowDetail(Relic relic)
    {
        relicDetailImage.sprite = relic.relicImage;
        relicDetailName.text = relic.relicName;
        relicDetailDescription1.text = relic.relicDescription1;
        relicDetailDescription2.text = relic.relicDescription2;
        DetailUI.SetActive(true);
    }
    
    public void BackButtonClicked()
    {
        DetailUI.SetActive(false);
    }

}
