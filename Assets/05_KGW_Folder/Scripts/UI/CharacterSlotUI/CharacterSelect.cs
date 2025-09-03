using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CJH;
using Unity.VisualScripting;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterDataSO _characterData;    // 캐릭터의 데이터
    private Button _selectButton;
    // CJH 코드 추가
    private TeamManager teamManager;

    private void Start()
    {
        _selectButton = GetComponent<Button>();
        _selectButton.onClick.AddListener(OnSelectClick);

        // CJH 코드 추가
        teamManager = FindObjectOfType<TeamManager>();

        // CJH 코드 추가
        if (teamManager == null)
        {
            Debug.LogError("씬에 TeamManager가 없습니다!");
            return;
        }

        GetComponent<Button>().onClick.AddListener(OnSelectClick);
    }

    private void OnDestroy()
    {
        _selectButton.onClick.RemoveListener(OnSelectClick);
    }

    // 캐릭터 선택
    private void OnSelectClick()
    {
        // 매니저에 선택한 캐릭터의 데이터 전달
        CharacterSelectManager.Instance.CharacterSelect(_characterData);

        //CJH 코드 추가
        if (TeamManager.Instance != null && _characterData != null)
        {
            // TeamManager에 선택된 캐릭터의 SO를 전달하여 팀에 추가하도록 요청
            TeamManager.Instance.AddCharacterBySO(_characterData);

            Debug.Log($"[CharacterSelect] {_characterData._chaBaseData.ChaName} 선택. TeamManager에 추가 요청 완료.");
        }
        else
        {
            Debug.LogError("[CharacterSelect] TeamManager.Instance 또는 CharacterDataSO가 할당되지 않았습니다!");
        }
    }
}
