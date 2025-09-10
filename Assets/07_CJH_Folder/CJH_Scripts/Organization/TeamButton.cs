using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TeamButton : MonoBehaviour
{
    [Header("연결할 UI 패널")]
    [Tooltip("버튼을 눌렀을 때 활성화시킬 팀 편성 UI 패널을 여기에 연결하세요.")]
    [SerializeField] private GameObject teamFormationPanel;

    void Start()
    {
        Button button = GetComponent<Button>();

        // 버튼 클릭 시 OpenPanel 함수가 호출되도록 이벤트를 등록
        button.onClick.AddListener(OpenPanel);

        // teamFormationPanel이 연결되었는지 확인
        if (teamFormationPanel == null)
        {
            Debug.LogError("TeamButtonHandler: teamFormationPanel 변수가 연결되지 않았습니다 인스펙터에서 연결해주세요.", this.gameObject);
        }
    }

    /// <summary>
    /// 팀 편성 패널을 활성화합니다.
    /// </summary>
    private void OpenPanel()
    {
        if (teamFormationPanel != null)
        {
            teamFormationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("팀 편성 패널이 연결되지 않아 열 수 없습니다", this.gameObject);
        }
    }
}