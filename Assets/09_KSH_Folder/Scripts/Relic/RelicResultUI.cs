using System.Collections.Generic;
using KSH;
using SDW;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicResultUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private RelicUI relicPrefab;
    
    [SerializeField] private Button getButton;
    [SerializeField] private GameObject DetailUI;
    [SerializeField] public GameObject RelicWindow;
    [SerializeField] private TextMeshProUGUI ignoreText;
    
    private RelicUI currentRelicUI;
    private RelicDatas relic;
    [SerializeField] private RelicDetailUI relicDetailUI;

    private void Awake()
    {
        getButton.onClick.AddListener(GetRelicClicked);
    }

    private void GetRelicClicked()
    {
        if (currentRelicUI == null)
        {
            ignoreText.gameObject.SetActive(true);
            return;
        }
        
        RelicDatas relic = currentRelicUI.GetRelic();
        RelicDropManager.Instance.GetRelic(relic);
        getButton.interactable = false;
        RelicWindow.SetActive(false);
    }
    
    public void ShowRelic(List<RelicDatas> relics, RelicGrade grade)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < relics.Count; i++)
        {
            var relic = Instantiate(relicPrefab, content);
            relic.SetData(relics[i], OnClickRelic, relicDetailUI);
        }
        currentRelicUI = null;
    }

    private void OnClickRelic(RelicUI relicUI)
    {
        currentRelicUI = relicUI;
    }
}
