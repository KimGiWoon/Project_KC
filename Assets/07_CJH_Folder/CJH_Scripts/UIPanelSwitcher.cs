using UnityEngine;
using UnityEngine.UI;

public class UIPanelSwitcher : MonoBehaviour
{
    [Header("전환할 패널들")]
    public GameObject nodeSelectPanel;      // 노드 선택창 (SelectonPanel)
    public GameObject characterSelectPanel; // 캐릭터 선택창

    [Header("전환 버튼")]
    public Button switchButton;             // UI 전환을 담당할 버튼

    // 게임 시작 시 초기 상태를 설정
    void Start()
    {
        // 처음에는 노드 선택창만 보이도록 설정
        nodeSelectPanel.SetActive(true);
        characterSelectPanel.SetActive(false);

        // 버튼이 눌렸을 때 TogglePanels 함수가 실행되도록 연결
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(TogglePanels);
        }
    }

    // 두 패널의 활성화 상태를 서로 전환하는 함수
    public void TogglePanels()
    {
        // 현재 활성화된 패널을 확인하고, 상태를 전환
        bool isNodePanelActive = nodeSelectPanel.activeSelf;

        nodeSelectPanel.SetActive(!isNodePanelActive);
        characterSelectPanel.SetActive(isNodePanelActive);
    }
}