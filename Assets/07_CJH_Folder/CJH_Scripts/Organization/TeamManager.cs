using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using SDW;

namespace CJH
{
    public class TeamManager : MonoBehaviour
    {
        public static TeamManager Instance;

        [Header("팀 슬롯 연결")]
        public TeamSlotClick[] teamSlots = new TeamSlotClick[3];

        [Header("모든 캐릭터 데이터")]
        public List<CharacterData> allCharacters;

        // 단일 팀 데이터 배열
        private CharacterData[] currentTeam = new CharacterData[3];

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }



        //todo 가챠에서 캐릭터 데이터 뽑히면 자동 추가 동작 되는지 확인 및 추가 시 동작 확인

        /// <summary>
        /// CharacterDataSO를 받아 팀에 캐릭터를 추가합니다.
        /// </summary>
        public void AddCharacterBySO(CharacterDataSO characterSO)
        {
            if (characterSO == null) return;

            // allCharacters 리스트에서 이름이 같은 CharacterData를 찾습니다.
            CharacterData characterToAdd = allCharacters.FirstOrDefault(c => c.characterName == characterSO._chaBaseData.ChaName);

            if (characterToAdd != null)
            {
                // 찾은 CharacterData를 팀에 추가합니다.
                AddCharacterToTeam(characterToAdd);
            }
            else
            {
                Debug.LogError($"{characterSO._chaBaseData.ChaName}에 해당하는 CharacterData를 'allCharacters' 리스트에서 찾을 수 없습니다.");
            

        }

        /// <summary>
        /// 캐릭터를 팀에 추가하는 로직
        /// </summary>
        private void AddCharacterToTeam(CharacterData characterToAdd)
        {
            // 이미 팀에 있는지 확인
            if (currentTeam.Contains(characterToAdd))
            {
                Debug.Log($"{characterToAdd.characterName}은(는) 이미 팀에 포함되어 있습니다.");
                return;
            }

            // 빈 슬롯 찾기
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

        /// <summary>
        /// 팀에서 캐릭터를 제거합니다.
        /// </summary>
        public void RemoveCharacterFromTeam(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= currentTeam.Length || currentTeam[slotIndex] == null) return;

            Debug.Log($"{currentTeam[slotIndex].characterName}을(를) 팀에서 제거했습니다.");
            currentTeam[slotIndex] = null;
            UpdateAllSlotsUI();
        }

        /// <summary>
        /// 팀이 3명으로 구성되었는지 확인합니다.
        /// </summary>
        public bool IsTeamFull()
        {
            int memberCount = currentTeam.Count(c => c != null);
            bool isFull = memberCount == 3;
            Debug.Log($"IsTeamFull Check: 현재 팀원 {memberCount}명. ");
            return isFull;
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
                    teamSlots[i].UpdateSlot(currentTeam[i]);
                }
            }
        }
    }
}