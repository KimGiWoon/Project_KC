using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private CharacterDataSO _characterData; // 캐릭터의 데이터
    private Button _selectButton;

    private void Start()
    {
        _selectButton = GetComponent<Button>();
        _selectButton.onClick.AddListener(OnSelectClick);
    }

    private void OnSelectClick()
    {
        // 매니저에 선택한 캐릭터의 데이터 전달
        // bool isOwned = SDW.GameManager.Instance.Reward.ownedCharacters.ContainsKey(_characterData._chaBaseData.ChaName);
        bool isOwned = SDW.GameManager.Instance.CharacterData.OwnedCharacters.ContainsKey(_characterData._chaBaseData.ChaName);
    }
}