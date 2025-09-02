using UnityEngine;
using System.Linq;
using System;

namespace CJH
{
    public class TeamManager : MonoBehaviour
    {
        public static TeamManager Instance;

        [Header("팀 슬롯 연결")]
        public TeamSlotClick[] teamSlots = new TeamSlotClick[3];

        private CharacterData[] committedTeam = new CharacterData[3];
        private CharacterData[] pendingTeam = new CharacterData[3];

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void OnPanelOpen()
        {
            Array.Copy(committedTeam, pendingTeam, committedTeam.Length);
            UpdateAllSlotsUI();
        }

        public void OnConfirmChanges()
        {
            Array.Copy(pendingTeam, committedTeam, pendingTeam.Length);
            Debug.Log("팀 편성이 적용되었습니다.");
        }

        public void OnCancel()
        {
            Array.Copy(committedTeam, pendingTeam, committedTeam.Length);
            UpdateAllSlotsUI();
            Debug.Log("편성을 취소하고 데이터를 원래 상태로 복구했습니다.");
        }


        //todo pendingTeam에 정보 전달 해줘야함. 현재 정보 전달이 안돼서 동작 안됨.
        // 팀이 3명으로 구성되었는지 확인하는 메서드
        public bool IsTeamFull()
        {
            return pendingTeam.All(character => character != null);
        }

        public void AddCharacterToTeam(CharacterData characterToAdd)
        {
            if (pendingTeam.Contains(characterToAdd)) return;
            int emptySlotIndex = Array.IndexOf(pendingTeam, null);
            if (emptySlotIndex != -1)
            {
                pendingTeam[emptySlotIndex] = characterToAdd;
                UpdateAllSlotsUI();
            }
        }

        public void RemoveCharacterFromTeam(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= pendingTeam.Length || pendingTeam[slotIndex] == null) return;
            pendingTeam[slotIndex] = null;
            UpdateAllSlotsUI();
        }

        private void UpdateAllSlotsUI()
        {
            for (int i = 0; i < teamSlots.Length; i++)
            {
                if (teamSlots[i] != null)
                {
                    teamSlots[i].UpdateSlot(pendingTeam[i]);
                }
            }
        }
    }
}