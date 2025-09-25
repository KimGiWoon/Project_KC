using System.Collections.Generic;
using SDW;
using UnityEngine;

public class EerieDarknessController : MonoBehaviour
{
    private float _attackUpValue;
    private float _activeSkillDuration;
    private MonsterController _monster;

    private Dictionary<MonsterController, float> _originalAttackValue = new Dictionary<MonsterController, float>();

    public void Init(MonsterController caster, float value, float duration)
    {
        _activeSkillDuration = duration;
        _attackUpValue = caster._monsterState._monAttack * value;
        _monster = caster;

        AllMonsterAttackUp();
    }

    // 전체 몬스터의 공격력 상승
    public void AllMonsterAttackUp()
    {
        // 기괴한 어둠 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.ShadowScarecrowSkill);

        foreach (var mon in _monster._battleManager._monsters)
        {
            if (!mon._isAlive) continue;

            // 몬스터의 원래 공격력 저장
            _originalAttackValue[mon] = mon._monsterState._monAttack;

            mon._monsterState._monAttack += _attackUpValue;
        }

        Invoke(nameof(CharacterAttackReset), _activeSkillDuration / _monster._gameSpeed);

        // 게임 종료가 되면 상승된 공격력 원복
        if (_monster._battleManager._isGameOver)
        {
            CharacterAttackReset();
        }
    }

    // 지속시간 후 상승된 공격력 원복
    public void CharacterAttackReset()
    {
        foreach (var mon in _monster._battleManager._monsters)
        {
            if (_originalAttackValue.ContainsKey(mon))
            {
                mon._monsterState._monAttack = _originalAttackValue[mon];
            }
        }

        Destroy(gameObject);
    }
}
