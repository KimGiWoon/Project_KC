using System.Collections;
using System.Collections.Generic;
using JJY;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelUpSceneManager : MonoBehaviour
{
    [Header("User Info")]
    [SerializeField] TextMeshProUGUI userLevel; // 현재 레벨
    [SerializeField] TextMeshProUGUI curExp;    // 현재 경험치 수치 (현재 경험치 / 최대 경험치)
    [SerializeField] TextMeshProUGUI addedExp;  // 아이템을 사용해서 얻는 경험치 수치
    [SerializeField] Image expBar;              // 현재 경험치 바

    [Header("Item Count Text")]
    [SerializeField] TextMeshProUGUI beeksCount;        // beek's의 수량
    [SerializeField] TextMeshProUGUI finediningCount;   // fine dining의 수량
    [SerializeField] TextMeshProUGUI masterChefCount;   // masterChef의 수량
    [SerializeField] TextMeshProUGUI hasItemCount;      // 선택된 아이템의 보유량
    [SerializeField] TextMeshProUGUI useItemCount;      // 선택된 아이템의 사용량 (최소 : 1)

    [Header("Dialog Text")]
    [SerializeField] TextMeshProUGUI dialog;    // dialog 표시

    [Header("Button")]
    [SerializeField] Button backBtn;    // 돌아가기 버튼

    void Start()
    {
        InitUserInfo();
        InitItemCountText();
    }

    /// <summary>
    /// 현재 보유중인 아이템의 수량을 표기하기 위한 초기화 작업
    /// </summary>
    private void InitItemCountText()
    {
        beeksCount.text = CoinManager.Instance.GetRecipeItemCount(CoinManager.Instance.beek).ToString();
        finediningCount.text = CoinManager.Instance.GetRecipeItemCount(CoinManager.Instance.fineDining).ToString();
        masterChefCount.text = CoinManager.Instance.GetRecipeItemCount(CoinManager.Instance.masterChef).ToString();
    }

    /// <summary>
    /// 유저 정보(경험치) 초기화 작업
    /// </summary>
    private void InitUserInfo()
    {
        // float currentExp = float.Parse(curExp.text);
        // float maximumExp = float.Parse(maxExp.text);
        // // 만렙일 경우 0
        // expBar.fillAmount = maximumExp > 0 ? currentExp / maximumExp : 0f;
    }
}
