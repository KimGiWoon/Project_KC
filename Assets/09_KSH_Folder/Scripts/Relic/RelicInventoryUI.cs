using System.Collections;
using System.Collections.Generic;
using KSH;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicInventoryUI : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject relicPrefab;
    [SerializeField] private InGameItemManager inGameItemManager;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private TextMeshProUGUI relicNameText;

    private void Awake()
    {
        inventoryButton.onClick.AddListener(RelicInventoryOpen);
        inventoryUI.SetActive(false);
    }
    private void OnEnable()
    {
        inGameItemManager.OnItemChanged += RelicUIAdd;
    }

    private void OnDisable()
    {
        inGameItemManager.OnItemChanged -= RelicUIAdd;   
    }

    private void RelicUIAdd()
    {
        foreach (Transform child in Content)
            Destroy(child.gameObject);

        foreach (var r in inGameItemManager.relicInventory)
        {
            GameObject relic = Instantiate(relicPrefab, Content);
            TextMeshProUGUI relicNameText = relic.GetComponentInChildren<TextMeshProUGUI>();
            if(relicNameText != null)
                relicNameText.text = r.relic.relicName;
        }
    }

    private void RelicInventoryOpen()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);;
    }
}
