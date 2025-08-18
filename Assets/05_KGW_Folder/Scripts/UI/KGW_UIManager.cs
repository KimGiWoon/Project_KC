using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KGW_UIManager : MonoBehaviour
{
    [Header("UI Setting")]
    [SerializeField] GameObject _slotPanel; // 슬롯 패널
    [SerializeField] Button _backButton;    // 뒤로가기 버튼
    [SerializeField] Button _okButton;      // 확인 버튼
    [SerializeField] Button _selectButton;  // 배치 버튼
    [SerializeField] Button _bettleButton;  // 전투 버튼

    private void Start()
    {
        _backButton.onClick.AddListener(() => OnBackButtonClick());
        _okButton.onClick.AddListener(() => OnOkButtonClick());
        _selectButton.onClick.AddListener(() => OnSelectButtonClick());
        _bettleButton.onClick.AddListener(() => OnBettleStartClick());
    }

    // 뒤로가기 버튼 클릭
    private void OnBackButtonClick()
    {
        _slotPanel.SetActive(false);
    }

    // 완료 버튼 클릭
    private void OnOkButtonClick()
    {
        // 전투 가능 확인
        if (!CharacterSelectManager.Instance._canBettle)
        {
            Debug.Log("전투 참가 인원이 부족합니다.");
        }
        else
        {
            _slotPanel.SetActive(false);
        }
    }

    // 인원 배치 버튼 클릭
    private void OnSelectButtonClick()
    {
        _slotPanel.SetActive(true);
    }

    // 전투 스타트
    public void OnBettleStartClick()
    {
        if (!CharacterSelectManager.Instance._canBettle)
        {
            Debug.Log("전투 참가 인원이 부족합니다.");
        }
        else
        {
            // 전투 시작
            SceneManager.LoadScene("KGW_TestIngameScene");
        }
    }
}
