using System.Collections;
using System.Collections.Generic;
using SDW;
using UnityEngine;

namespace KGW
{
    public class CharacterBattleDataSaveManager : MonoBehaviour
    {
        // 캐릭터의 체력 저장 딕셔너리
        public Dictionary<CharacterEnName, float> _chaHpSave = new Dictionary<CharacterEnName, float>();

        // TODO : 보유중인 유물과 요리도 저장
    }
}

