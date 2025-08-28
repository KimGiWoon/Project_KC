using System.Collections;
using System.Collections.Generic;
using KSH;
using TMPro;
using UnityEngine;

public class RelicResultUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private RelicUI relicPrefab;
    
    [SerializeField] private TextMeshProUGUI relicDescription1;
    [SerializeField] private TextMeshProUGUI relicDescription2;
    
    private RelicUI currentRelicUI;
    
    public void ShowRelic(List<Relic> relics)
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
