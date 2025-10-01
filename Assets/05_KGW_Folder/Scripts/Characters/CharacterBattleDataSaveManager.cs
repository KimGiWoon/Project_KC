using System.Collections;
using System.Collections.Generic;
using System.Data;
using SDW;
using Unity.VisualScripting;
using UnityEngine;

namespace KGW
{
    public class CharacterBattleDataSaveManager : MonoBehaviour
    {
        // 캐릭터의 레벨
        public Dictionary<CharacterEnName, int> _chaLevel = new Dictionary<CharacterEnName, int>();

        // 캐릭터의 업그레이드 레벨
        public Dictionary<CharacterEnName, int> _chaUpgrade = new Dictionary<CharacterEnName, int>();

        // 캐릭터의 현재 체력
        public Dictionary<CharacterEnName, float> _chaHpSave = new Dictionary<CharacterEnName, float>();
        public Dictionary<CharacterEnName, float> _chaCalculatedHpSave = new Dictionary<CharacterEnName, float>();

        // 캐릭터의 최대 체력
        public Dictionary<CharacterEnName, float> _chaMaxHpSave = new Dictionary<CharacterEnName, float>();

        private void Start()
        {
            GameManager.Instance.Firebase.OnUserInfoUpdated += ClearDictionary;
        }

        // TODO : 보유중인 유물과 요리도 저장
        private void ClearDictionary()
        {
            _chaLevel.Clear();
            _chaUpgrade.Clear();
            _chaHpSave.Clear();
            _chaCalculatedHpSave.Clear();
            _chaMaxHpSave.Clear();
        }
    }
}