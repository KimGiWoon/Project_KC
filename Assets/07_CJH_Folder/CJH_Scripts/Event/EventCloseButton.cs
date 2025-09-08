using CJH;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 닫기 버튼 자동 할당 및 이벤트 종료 기능 추가
/// </summary>
public class EventCloseButton : MonoBehaviour
{
    // 스크립트가 시작될 때 한 번 실행
    void Start()
    {
        // 이 게임 오브젝트와 모든 자식 오브젝트에 있는 Button 컴포넌트를 할당
        Button[] allButtons = GetComponentsInChildren<Button>(true); // true는 비활성화된 오브젝트의 버튼도 포함

        // 찾은 모든 버튼에 대해 반복 작업을 진행
        foreach (Button button in allButtons)
        {
            // 각 버튼의 OnClick 이벤트에 EndEncounterViaManager 함수를 리스너로 등록
            // 이렇게 하면 어떤 버튼을 눌러도 이벤트 종료 함수가 호출
            button.onClick.AddListener(EndEncounterViaManager);
        }

        Debug.Log($"'{gameObject.name}' 패널에서 총 {allButtons.Length}개의 버튼에 닫기 기능을 자동으로 연결했습니다.");
    }

    /// <summary>
    /// EventManager를 호출하여 이벤트를 종료하는 실제 로직이 담긴 함수입니다.
    /// </summary>
    private void EndEncounterViaManager()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.EndEncounter();
        }
        else
        {
            Debug.LogError("EventManager 인스턴스를 찾을 수 없습니다");
        }
    }
}