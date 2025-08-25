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
            if (RewardChangeManager.Instance.starCandy >= 150)
            {
                GatchaUI.SetActive(false);
                RewardChangeManager.Instance.starCandy -= 150;
                gacha.SingleGacha();
            }
            else
            {
                Debug.Log("별사탕이 부족합니다.");
            }
        });
        multipleButton.onClick.AddListener(() =>
        {
            if (RewardChangeManager.Instance.starCandy >= 1500)
            {
                GatchaUI.SetActive(false);
                RewardChangeManager.Instance.starCandy -= 1500;
                gacha.TenGacha();    
            }
            else
            {
                Debug.Log("별사탕이 부족합니다.");
            }
        });
    }
}
