using System.Collections;
using UnityEngine;

public class SavageRushController : MonoBehaviour
{
    private float _skillDamage;
    private MyCharacterController _character;
    private MonsterController _monster;
    private Vector3 _position;
    private Coroutine _rushRoutine;

    public void Init(MonsterController caster, MyCharacterController target, float damageValue)
    {
        _skillDamage = caster._monsterState._monAttack * damageValue;
        _character = target;
        _monster = caster;
        _position = caster.transform.position;

        CharacterSavageRush();
    }

    // 공격 대상인 캐릭터에게 난폭한 돌진 사용
    public void CharacterSavageRush()
    {
        if (!_character._isAlive) return;

        if (_character._battleManager._isGameOver)
        {
            RushCoroutineStop();
        }

        //_rushRoutine = StartCoroutine(RushCoroutine());

        // 2초의 대기 시간 후 돌진 사용
        Invoke(nameof(StartRush), 2f);
    }

    // 돌진 시작
    private void StartRush()
    {
        _rushRoutine = StartCoroutine(RushCoroutine());
    }

    private IEnumerator RushCoroutine()
    {
        Vector3 startPos = _position;
        Vector3 endPos;
        Vector3 rushDir = Vector3.right;
        float rushTime = 0.5f;
        float timer = 0f;
        float returnDuration = 0.5f;

        // 그로기 상태에서는 돌진 금지
        if (_monster._isStern)
        {
            yield break;
        }

        // 앞으로 돌진
        while (timer < rushTime)
        {
            _monster.transform.position += rushDir * _monster._monsterState._monMoveSpeed * 10f * Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }

        endPos = _monster.transform.position;
        timer = 0f;

        // 공격 대상의 캐릭터 공격
        _character.TakeDamage(_skillDamage, _monster._monsterState._monAccuracy);

        // 돌진 전 위치로 돌아오기
        while (timer < returnDuration)
        {
            _monster.transform.position = Vector3.Lerp(endPos, startPos, timer /  returnDuration);
            timer += Time.deltaTime;

            yield return null;
        }

        // 복귀 위치 고정
        _monster.transform.position = startPos;

        Destroy(gameObject);
    }

    // 코루틴 정지
    private void RushCoroutineStop()
    {
        if(_rushRoutine != null)
        {
            StopCoroutine(_rushRoutine);
            _rushRoutine = null;
        }
    }
}
