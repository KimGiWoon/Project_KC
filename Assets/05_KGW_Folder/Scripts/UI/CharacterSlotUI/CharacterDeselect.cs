using UnityEngine;
using UnityEngine.UI;

public class CharacterDeselect : MonoBehaviour
{
    CharacterDataSO _characterData;    // 캐릭터의 데이터

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnDeselectClick);
    }

    // 캐릭터의 데이터 받기
    public void SetData(CharacterDataSO Data)
    {
        _characterData = Data;
    }

    // 선택한 캐릭터 취소
    private void OnDeselectClick()
    {
        // 매니저에 선택한 캐릭터의 취소와 데이터 전달
        CharacterSelectManager.Instance.CharacterDeselect(_characterData);
        Destroy(gameObject);
    }
}
