using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KSH
{
    public class GachaMainUI : MonoBehaviour
    {
        [Header("CharacterGacha")]
        [SerializeField] private CharacterGacha gacha;
        [Header("버튼")]
        [SerializeField] private Button singleButton; //1회 뽑기 버튼
        [SerializeField] private Button multipleButton; //10회 뽑기 버튼
        [Header("메인UI")]
        [SerializeField] private GameObject GatchaUI; //메인 UI
        [Header("별사탕 UI")]
        [SerializeField] private TextMeshProUGUI starCandyText;
        
        public int starCandyCount { get; private set; }

        private void Start()
        {
            singleButton.onClick.AddListener(() =>
            {
                if (RewardChangeManager.Instance.StarCandy >= 150) //별사탕이 150개 이상 가지고 있으면 1회 뽑기
                {
                    GatchaUI.SetActive(false);
                    RewardChangeManager.Instance.AddStarCandy(-150);
                    gacha.SingleGacha();
                }
                else
                {
                    Debug.Log("별사탕이 부족합니다.");
                }
            });
            multipleButton.onClick.AddListener(() =>
            {
                if (RewardChangeManager.Instance.StarCandy >= 1500) //별사탕을 1500개 이상 가지고 있으면 10회 뽑기
                {
                    GatchaUI.SetActive(false);
                    RewardChangeManager.Instance.AddStarCandy(-1500);
                    gacha.TenGacha();    
                }
                else
                {
                    Debug.Log("별사탕이 부족합니다.");
                }
            });
        }

        private void OnEnable()
        {
            RewardChangeManager.Instance.OnStarCandyChange += CandyUpdate;
            CandyUpdate(RewardChangeManager.Instance.StarCandy);
        }

        private void OnDisable()
        {
            if (RewardChangeManager.Instance != null)
                RewardChangeManager.Instance.OnStarCandyChange -= CandyUpdate;
        }
        
        private void CandyUpdate(int value)
        {
            starCandyText.text = value.ToString();
        }
    }
    
}