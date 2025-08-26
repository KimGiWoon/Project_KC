using UnityEngine;
using UnityEngine.UI;

namespace KSH
{
    public class GachaButton : MonoBehaviour
    {
        [Header("CharacterGacha")]
        [SerializeField] private CharacterGacha gacha;
        [Header("버튼")]
        [SerializeField] private Button singleButton; //1회 뽑기 버튼
        [SerializeField] private Button multipleButton; //10회 뽑기 버튼
        [Header("메인UI")]
        [SerializeField] private GameObject GatchaUI; //메인 UI

        private void Start()
        {
            singleButton.onClick.AddListener(() =>
            {
                if (RewardChangeManager.Instance.starCandy >= 150) //별사탕이 150개 이상 가지고 있으면 1회 뽑기
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
                if (RewardChangeManager.Instance.starCandy >= 1500) //별사탕을 1500개 이상 가지고 있으면 10회 뽑기
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
    
}