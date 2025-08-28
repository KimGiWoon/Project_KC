using System.Collections.Generic;
using KSH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicResultUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private RelicUI relicPrefab;
    
    [SerializeField] private TextMeshProUGUI relicDescription1;
    [SerializeField] private TextMeshProUGUI relicDescription2;
    [SerializeField] private Image Background;
    
    private RelicUI currentRelicUI;
    private Relic relic;
    
    public void ShowRelic(List<Relic> relics, RelicRarity rarity)
    {
        relicDescription1.gameObject.SetActive(false);
        relicDescription2.gameObject.SetActive(false);
        
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < relics.Count; i++)
        {
            var relic = Instantiate(relicPrefab, content);
            relic.SetData(relics[i], OnClickRelic);
            relic.SetOutline(false);
        }
        
        switch (rarity)
        {
            case RelicRarity.Normal:
                Background.color = Color.white;
                break;
            case RelicRarity.Rare:
                Background.color = Color.yellow;
                break;
            case RelicRarity.Deburff:
                Background.color = Color.magenta;
                break;
        }
        
        currentRelicUI = null;
    }

    private void OnClickRelic(RelicUI relicUI)
    {
        if(currentRelicUI != null)
            currentRelicUI.SetOutline(false);
        
        currentRelicUI = relicUI;
        currentRelicUI.SetOutline(true);
        
        relicDescription1.gameObject.SetActive(true);
        relicDescription2.gameObject.SetActive(true);

        Relic relic = currentRelicUI.GetRelic();
        relicDescription1.text = relic.relicDescription1;
        relicDescription2.text = relic.relicDescription2;
    }
}
