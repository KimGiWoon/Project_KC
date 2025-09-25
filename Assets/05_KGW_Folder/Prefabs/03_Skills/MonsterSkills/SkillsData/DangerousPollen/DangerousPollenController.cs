using SDW;
using UnityEngine;

public class DangerousPollenController : MonoBehaviour
{
    private float _healValue;
    private MonsterController _monster;

    public void Init(MonsterController caster, float value, float duration)
    {
        _healValue = caster._monsterState._monAttack * value;
        _monster = caster;

        AllMonsterHpHeal();
    }

    // 전체 몬스터의 체력 회복
    public void AllMonsterHpHeal()
    {
        // 위험한 꽃가루 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.CorruptedFlowerSkill);

        foreach (var mon in _monster._battleManager._monsters)
        {
            if (!mon._isAlive) continue;

            mon.MonsterHealApply(_healValue);

        }

        Destroy(gameObject);
    }
}
    

