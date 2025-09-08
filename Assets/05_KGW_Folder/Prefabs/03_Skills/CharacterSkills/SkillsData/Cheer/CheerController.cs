using UnityEngine;

public class CheerController : MonoBehaviour
{
    private float _attackUpValue;
    private float _saveAttack;
    private float _activeSkillDuration;
    private bool _isAttackUp;
    private MyCharacterController _character;

    public void Init(MyCharacterController caster, float value, float duration)
    {
        _attackUpValue = value;
        _activeSkillDuration = duration;
        _saveAttack = caster._characterState._chaAttack;
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

            cha._characterState._chaAttack += _attackUpValue;
            Debug.Log($"{cha._characterState._chaEnName}의 공격력이 {_attackUpValue}만큼 상승했습니다.");
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
            cha._characterState._chaAttack = _saveAttack;
            Debug.Log($"{cha._characterState._chaEnName}의 공격력이 원상복귀 되었습니다.");

            _isAttackUp = false;
        }
    }
}
