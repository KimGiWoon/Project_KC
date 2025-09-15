using System.Collections;
using System.Collections.Generic;
using System.Data;
using SDW;
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

        // TODO : 보유중인 유물과 요리도 저장
    }
}

