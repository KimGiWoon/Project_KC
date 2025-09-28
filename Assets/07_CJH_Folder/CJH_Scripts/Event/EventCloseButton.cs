using CJH;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 닫기 버튼 자동 할당 및 이벤트 종료 기능 추가
/// </summary>
public class EventCloseButton : MonoBehaviour
{
    [SerializeField] private EventManager _eventManager;

    // 스크립트가 시작될 때 한 번 실행
    private void Start()
    {
        _eventManager = FindObjectOfType<EventManager>();

        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(EndEncounterViaManager);
            // Debug.Log($"'{gameObject.name}' 버튼에 닫기 기능을 연결했습니다.");
        }
        // else
        // {
        //     Debug.LogError("EventCloseButton이 붙은 오브젝트에 Button 컴포넌트가 없습니다!");
        // }
    }

    /// <summary>
    /// EventManager를 호출하여 이벤트를 종료하는 실제 로직이 담긴 함수입니다.
    /// </summary>
    private void EndEncounterViaManager()
    {
        if (_eventManager != null)
        {
            _eventManager.EndEncounter();
        }
        else
        {
            Debug.LogError("EventManager 인스턴스를 찾을 수 없습니다");
        }
    }
}