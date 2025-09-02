using UnityEngine;
using UnityEngine.UI;

public class UIPanelSwitcher : MonoBehaviour
{
    [Header("전환할 패널들")]
    public GameObject nodeSelectPanel;
    public GameObject characterSelectPanel;

    [Header("전환 버튼")]
    public Button switchButton;

    void Start()
    {
        // 시작 시 노드 선택창으로 설정
        ShowNodePanel();

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(TogglePanels);
        }
    }

    // 두 패널의 활성화 상태를 서로 전환
    public void TogglePanels()
    {
        bool isNodePanelActive = nodeSelectPanel.activeSelf;
        nodeSelectPanel.SetActive(!isNodePanelActive);
        characterSelectPanel.SetActive(isNodePanelActive);
    }

    // 캐릭터 선택창(편성창)을 보여주는 함수
    public void ShowCharacterPanel()
    {
        nodeSelectPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    // 노드 선택창을 보여주는 함수
    public void ShowNodePanel()
    {
        nodeSelectPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
    }

    // 스위치 버튼의 활성화 상태를 설정하는 함수
    public void SetSwitchButtonActive(bool isActive)
    {
        if (switchButton != null)
        {
            switchButton.gameObject.SetActive(isActive);
        }
    }
}