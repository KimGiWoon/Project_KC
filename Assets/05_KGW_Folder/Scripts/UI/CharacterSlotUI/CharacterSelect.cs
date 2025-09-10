using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CJH;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;
using Gley.Jumpy;
using KSH;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] CharacterDataSO _characterData;    // 캐릭터의 데이터
    private Button _selectButton;

    private void Start()
    {
        _selectButton = GetComponent<Button>();
        _selectButton.onClick.AddListener(OnSelectClick);

    }


    private void OnSelectClick()
    {
        // 매니저에 선택한 캐릭터의 데이터 전달
       bool isOwned = SDW.GameManager.Instance.Reward.ownedCharacters.ContainsKey(_characterData._chaBaseData.ChaName);
      

    }
}
