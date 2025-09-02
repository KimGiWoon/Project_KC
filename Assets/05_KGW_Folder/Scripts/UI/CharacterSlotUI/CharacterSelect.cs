using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CJH;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterDataSO _characterData;    // 캐릭터의 데이터

    // CJH 코드 추가
    private TeamManager teamManager;

    private void Start()
    {
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



    // 캐릭터 선택
    private void OnSelectClick()
    {
        // 매니저에 선택한 캐릭터의 데이터 전달
        CharacterSelectManager.Instance.CharacterSelect(_characterData);

        // CJH 코드 추가
        Debug.Log($"{_characterData._characterName}을 선택했습니다.");

    }
}
