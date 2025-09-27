using System.Collections;
using SDW;
using UnityEngine;

public class SavageRushController : MonoBehaviour
{
    private float _skillDamage;
    private MyCharacterController _character;
    private MonsterController _monster;

    public void Init(MonsterController caster, MyCharacterController target, float damageValue)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _character = target;
        _monster = caster;

        CharacterSavageRush();
    }

    // 공격 대상인 캐릭터에게 난폭한 돌진 사용
    public void CharacterSavageRush()
    {
        if (!_character._isAlive) return;

        // 2초의 대기 시간 후 돌진 상호작용
        Invoke(nameof(RushPlay), 2f);
    }

    // 돌진 플레이
    private void RushPlay()
    {
        // 공격 대상의 캐릭터 공격
        _character.TakeDamage(_skillDamage, _monster._monsterState._monAccuracy);

        // 난폭한 돌진 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.IronBullSkill_2);

        Destroy(gameObject);
    }
}
