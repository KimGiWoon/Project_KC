using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaButton : MonoBehaviour
{
    [SerializeField] private CharacterGacha gacha;
    [Header("Buttons")]
    [SerializeField] private Button singleButton;
    [SerializeField] private Button multipleButton;
    [SerializeField] private GameObject GatchaUI;

    private void Start()
    {
        singleButton.onClick.AddListener(() =>
        {
            GatchaUI.SetActive(false);
            gacha.SingleGacha();
        });
        multipleButton.onClick.AddListener(() =>
        {
            GatchaUI.SetActive(false);
            gacha.TenGacha();
        });
    }
}
