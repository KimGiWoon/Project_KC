using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Organization : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject teamPanel;        // 편성 UI 전체
    public Button openButton;           // 편성 버튼
    public Button closeButton;          // X 버튼


    void Start()
    {
        // 초기 상태 설정
        teamPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        openButton.gameObject.SetActive(true);

        // 이벤트 연결
        openButton.onClick.AddListener(OpenTeamPanel);
        closeButton.onClick.AddListener(CloseTeamPanel);
    }

    public void OpenTeamPanel()
    {
        teamPanel.SetActive(true);            // 편성 창 열기
        openButton.gameObject.SetActive(false);  // 편성 버튼 숨기기
        closeButton.gameObject.SetActive(true);  // X 버튼 보이기
    }

    public void CloseTeamPanel()
    {
        teamPanel.SetActive(false);           // 편성 창 닫기
        openButton.gameObject.SetActive(true);   // 편성 버튼 보이기
        closeButton.gameObject.SetActive(false); // X 버튼 숨기기
    }
}