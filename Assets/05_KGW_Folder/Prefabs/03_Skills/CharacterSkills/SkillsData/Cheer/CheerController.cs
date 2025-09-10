using System.Collections.Generic;
using UnityEngine;

public class CheerController : MonoBehaviour
{
    private float _attackUpValue;
    private float _activeSkillDuration;
    private bool _isAttackUp;
    private MyCharacterController _character;

    private Dictionary<MyCharacterController, float> _originalAttackValue = new Dictionary<MyCharacterController, float>();

    public void Init(MyCharacterController caster, float value, float duration)
    {
        _attackUpValue = caster._characterState._chaAttack * value;
        _activeSkillDuration = duration;
        _isAttackUp = false;
        _character = caster;

        AllCharacterAttackUp();
    }

    // 전체 캐릭터의 공격력 상승
    public void AllCharacterAttackUp()
    {
        if (_isAttackUp) return;

        foreach (var cha in _character._battleManager._characters)
        {
            if (!cha._isAlive) continue;

            // 캐릭터의 원래 공격력 저장
            _originalAttackValue[cha] = cha._characterState._chaAttack;

            cha._characterState._chaAttack += _attackUpValue;
        }
        _isAttackUp = true;

        Invoke(nameof(CharacterAttackReset), _activeSkillDuration);

        // 게임 종료가 되면 상승된 공격력 원복
        if (_character._battleManager._isGameOver)
        {
            CharacterAttackReset();
        }
    }

    // 지속시간 후 상승된 공격력 원복 
    public void CharacterAttackReset()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            if (_originalAttackValue.ContainsKey(cha))
            {
                cha._characterState._chaAttack = _originalAttackValue[cha];
            }

            _isAttackUp = false;
        }

        Destroy(gameObject);
    }
}
