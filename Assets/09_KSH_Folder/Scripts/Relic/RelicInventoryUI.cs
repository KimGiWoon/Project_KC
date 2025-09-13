using System;
using System.Collections;
using System.Collections.Generic;
using KSH;
using SDW;
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

    private void Awake()
    {
        inventoryButton.onClick.AddListener(RelicInventoryOpen);
        inventoryUI.SetActive(false);
    }

    private void Start()
    {
        inGameItemManager = GameManager.Instance.InGameItem;
    }

    private void OnDestroy()
    {
        inventoryButton.onClick.RemoveListener(RelicInventoryOpen);
    }

    public void RelicUIAdd()
    {
        inventoryUI.SetActive(false);
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }

        if (inGameItemManager.relicInventory == null) return;

        foreach (var r in inGameItemManager.relicInventory)
        {
            var relic = Instantiate(relicPrefab, Content);
            var image = relic.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = r.relic.relicImage;
            }
        }
    }

    private void RelicInventoryOpen()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}