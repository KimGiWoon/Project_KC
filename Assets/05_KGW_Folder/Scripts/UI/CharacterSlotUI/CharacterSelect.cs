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

    }

    private void OnDestroy()
    {
        _selectButton.onClick.RemoveListener(OnSelectClick);
    }



     //캐릭터 선택
     //CJH 코드 추가
    private void OnSelectClick()
    {
        // 매니저에 선택한 캐릭터의 데이터 전달
       bool isOwned = SDW.GameManager.Instance.Reward.ownedCharacters.ContainsKey(_characterData._chaBaseData.ChaName);
      
       if (isOwned)
        {
            // 팀에 캐릭터를 추가
            if (TeamManager.Instance != null)
            {
                TeamManager.Instance.AddCharacterBySO(_characterData);
                Debug.Log($"[CharacterSelect] 보유 중인 '{_characterData._chaBaseData.ChaName}' 캐릭터를 팀에 추가합니다.");
            }
            else
            {
                Debug.LogError("[CharacterSelect] TeamManager 인스턴스를 찾을 수 없습니다!");
            }
      }
      else // 캐릭터를 보유하고 있지 않다면
      {
          // 팀에 추가하지 않고, 로그 출력
          Debug.LogWarning($"[CharacterSelect] '{_characterData._chaBaseData.ChaName}'는 보유하지 않은 캐릭터라 팀에 추가할 수 없습니다.");
      }
    }
}
