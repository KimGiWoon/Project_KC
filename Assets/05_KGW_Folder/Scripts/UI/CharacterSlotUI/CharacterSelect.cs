using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterDataSO _characterData;    // 캐릭터의 데이터

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnSelectClick);
    }

    // 캐릭터 선택
    private void OnSelectClick()
    {
        // 매니저에 선택한 캐릭터의 데이터 전달
        CharacterSelectManager.Instance.CharacterSelect(_characterData);
        Debug.Log($"{_characterData._characterName}을 선택했습니다.");
    }
}
