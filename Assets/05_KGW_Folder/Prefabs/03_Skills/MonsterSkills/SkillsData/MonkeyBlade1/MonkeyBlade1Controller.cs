using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class MonkeyBlade1Controller : MonoBehaviour
{
    private float _skillDamage;
    private MyCharacterController _character;
    private MonsterController _monster;
    private Coroutine _bladeRoputine;
    private SpriteRenderer _bladeSprite;

    public void Init(MonsterController caster, MyCharacterController target, float damageValue)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _character = target;
        _monster = caster;
        _bladeSprite = GetComponent<SpriteRenderer>();

        _bladeSprite.enabled = false;
        AllCharacterMonkeyBlade();
    }

    // 전체 캐릭터에게 원숭이 검술 사용
    public void AllCharacterMonkeyBlade()
    {
        if (!_character._isAlive) return;

        if (_character._battleManager._isGameOver)
        {
            BladeCoroutineStop();
        }

        Invoke(nameof(StartMonkeyBlade), 2.8f);
    }

    // 검술 시작
    private void StartMonkeyBlade()
    {
        _bladeRoputine = StartCoroutine(BladeCoroutine());
    }

    private IEnumerator BladeCoroutine()
    {
        Vector3 bladeDir = Vector3.left;
        float bladeTime = 1f;
        float timer = 0f;

        // 그로기 상태에서는 검술 금지
        if (_monster._isStern)
        {
            yield break;
        }

        _bladeSprite.enabled = true;

        while (timer < bladeTime)
        {
            gameObject.transform.position += bladeDir * 10f * Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }

        // 전체 캐릭터에게 원숭이 검술 사용
        foreach (var cha in _character._battleManager._characters)
        {
            if (!cha._isAlive) continue;

            cha.TakeDamage(_skillDamage, _monster._monsterState._monAccuracy);
        }

        Destroy(gameObject);
    }

    // 코루틴 정지
    private void BladeCoroutineStop()
    {
        if (_bladeRoputine != null)
        {
            StopCoroutine(_bladeRoputine);
            _bladeRoputine = null;
        }
    }
}
