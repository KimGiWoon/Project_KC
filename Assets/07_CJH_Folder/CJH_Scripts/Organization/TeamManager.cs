using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CJH
{
    public class TeamManager : MonoBehaviour
    {
        public static TeamManager Instance;

        [Header("UI 연결")]
        public TeamSlotClick[] teamSlots = new TeamSlotClick[3];
        public CharacterInfoPanel characterInfoPanel; // 캐릭터 정보 패널 연결

        [Header("모든 캐릭터 데이터")]
        public List<CharacterData> allCharacters;

        private CharacterData[] currentTeam = new CharacterData[3];

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// 모든 팀 슬롯의 UI를 현재 팀 상태에 맞게 업데이트합니다.
        /// </summary>
        private void UpdateAllSlotsUI()
        {
            for (int i = 0; i < teamSlots.Length; i++)
            {
                if (teamSlots[i] != null)
                {
                    // 기존 슬롯 UI 업데이트
                    teamSlots[i].UpdateSlot(currentTeam[i]);

                    // 롱 프레스 컴포넌트에 데이터 전달
                    TeamSlotLongPress longPress = teamSlots[i].GetComponent<TeamSlotLongPress>();
                    if (longPress != null)
                    {
                        longPress.Setup(currentTeam[i], characterInfoPanel);
                    }
                }
            }
        }

        public void AddCharacterBySO(CharacterDataSO characterSO)
        {
            if (characterSO == null) return;
            CharacterData characterToAdd = allCharacters.FirstOrDefault(c => c.characterName == characterSO._characterName);
            if (characterToAdd != null)
            {
                AddCharacterToTeam(characterToAdd);
            }
            else
            {
                Debug.LogError($"{characterSO._characterName}에 해당하는 CharacterData를 'allCharacters' 리스트에서 찾을 수 없습니다.");
            }
        }

        private void AddCharacterToTeam(CharacterData characterToAdd)
        {
            if (currentTeam.Contains(characterToAdd))
            {
                Debug.Log($"{characterToAdd.characterName}은(는) 이미 팀에 포함되어 있습니다.");
                return;
            }
            int emptySlotIndex = Array.IndexOf(currentTeam, null);
            if (emptySlotIndex != -1)
            {
                currentTeam[emptySlotIndex] = characterToAdd;
                Debug.Log($"{characterToAdd.characterName}을(를) 팀에 추가했습니다. 현재 팀원: {currentTeam.Count(c => c != null)}명");
                UpdateAllSlotsUI();
            }
            else
            {
                Debug.Log("팀이 가득 찼습니다.");
            }
        }

        public void RemoveCharacterFromTeam(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= currentTeam.Length || currentTeam[slotIndex] == null) return;
            Debug.Log($"{currentTeam[slotIndex].characterName}을(를) 팀에서 제거했습니다.");
            currentTeam[slotIndex] = null;
            UpdateAllSlotsUI();
        }

        public bool IsTeamFull()
        {
            int memberCount = currentTeam.Count(c => c != null);
            bool isFull = memberCount == 3;
            Debug.Log($"IsTeamFull Check: 현재 팀원 {memberCount}명. 가득 찼나? {isFull}");
            return isFull;
        }
    }
}