using UnityEngine;

public class CharacterAttackController : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] MyCharacterController _controller;

    // 몬스터와 보스 레이어
    int _monsterLayer;
    int _bossLayer;

    private void Awake()
    {
        _monsterLayer = LayerMask.NameToLayer("Monster");
        _bossLayer = LayerMask.NameToLayer("Boss");
    }

    // 사거리에 들어온 몬스터
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _monsterLayer || collision.gameObject.layer == _bossLayer)
        {
            MonsterController monster = collision.GetComponent<MonsterController>();

            // 공격 가능한 몬스터에 해당 몬스터가 없으면
            if (!_controller._attackTargets.Contains(monster))
            {
                // 감지된 몬스터 추가
                _controller._attackTargets.Add(monster);

                RecheckAttackTarget();

                // 현재 공격 대상이 없으면
                if (_controller._attackTarget == null)
                {
                    // 감지된 몬스터를 공격 대상에 지정
                    _controller._attackTarget = monster;
                }
                else // 공격 대상이 있지만
                {
                    // 공격 대상이 보스몬스터이고 감지된 몬스터가 몬스터이면 (몬스터 우선 공격)
                    if (_controller._attackTarget.gameObject.layer == _bossLayer && collision.gameObject.layer == _monsterLayer)
                    {
                        // 몬스터를 공격 타겟으로 설정
                        _controller._attackTarget = monster;
                    }   // 공격 대상이 보스몬스터이고 감지된 몬스터가 보스이면
                    else if (_controller._attackTarget.gameObject.layer == _bossLayer && collision.gameObject.layer == _bossLayer)
                    {
                        // 보스를 공격 타겟으로 설정
                        _controller._attackTarget = monster;
                    }
                }
            }
        }
    }

    // 공격 대상 재선정
    public void RecheckAttackTarget()
    {
        // 우선 공격 타겟
        MonsterController firstTarget = null;

        // 가장 가까운 거리 저장 변수 초기화
        float firstTargetDistance = float.PositiveInfinity;

        foreach (MonsterController monster in _controller._attackTargets)
        {
            // 몬스터가 없거나 죽었으면 넘어가기
            if (monster == null || !monster.isActiveAndEnabled) continue;

            // 몬스터와 거리 확인
            float distance = Vector3.SqrMagnitude(monster.transform.position - transform.position);

            if (firstTarget != null)
            {
                // 우선 공격 몬스터가 보스이지만 공격 대상에 몬스터가 있으면 
                if (firstTarget.gameObject.layer == _bossLayer && monster.gameObject.layer == _monsterLayer)
                {
                    // 보스보다 몬스터를 무조건 우선 공격 설정
                    firstTarget = monster;
                    firstTargetDistance = distance;
                    continue;
                }
            }

            // 몬스터는 더 가까운 몬스터 공격
            if (distance < firstTargetDistance)
            {
                firstTarget = monster;
                firstTargetDistance = distance;
            }
        }

        // 공격 대상에 타겟 설정
        _controller._attackTarget = firstTarget;
    }

    // 사거리에서 벗어난 몬스터
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _monsterLayer || collision.gameObject.layer == _bossLayer)
        {
            MonsterController monster = collision.GetComponent<MonsterController>();

            // 공격 가능한 몬스터에 해당 몬스터가 있으면
            if (_controller._attackTargets.Contains(monster))
            {
                // 몬스터 삭제
                _controller._attackTargets.Remove(monster);

                RecheckAttackTarget();

                // 현재의 타겟이 사거리에서 벗어나면
                if (_controller._attackTarget == monster)
                {
                    // 다음 순서의 몬스터가 있으면 현재 공격 대상으로 변경
                    _controller._attackTarget = _controller._attackTargets.Count > 0 ? _controller._attackTargets[0] : null;
                }
            }
        }
    }
}
