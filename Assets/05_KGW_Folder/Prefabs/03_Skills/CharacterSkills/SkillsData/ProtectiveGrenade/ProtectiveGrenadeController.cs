using SDW;
using UnityEngine;

public class ProtectiveGrenadeController : MonoBehaviour
{
    private float _skillDamage;
    private float _saveAttack;
    private MyCharacterController _character;
    private MonsterController _monster;

    public void Init(MyCharacterController caster, MonsterController target, float value)
    {
        _saveAttack = caster._characterState._chaAttack;
        _skillDamage = _saveAttack * value;
        _character = caster;
        _monster = target;

        AllMonsterGrenade();
    }

    // 전체 몬스터한테 수류탄 투척
    public void AllMonsterGrenade()
    {
        // 보호 수류탄 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.BCAActiveSkill);

        foreach (var mon in _monster._battleManager._monsters)
        {
            if (!mon._isAlive) continue;

            mon.TakeDamage(_skillDamage, _character._characterState._chaAccuracy);
        }

        AllCharacterBarrier();
    }

    // 전체 캐릭터한테 배리어 부여
    public void AllCharacterBarrier()
    {
        foreach (var cha in _character._battleManager._characters)
        {
            cha._characterState._isBarrier = true;
        }

        Destroy(gameObject);
    }
}
