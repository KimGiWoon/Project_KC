using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterResearchContoller : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] MyCharacterController _controller;

    // 몬스터와 보스 레이어
    int _monsterLayer;
    int _bossLayer;

    private void Awake()
    {
        // 몬스터와 보스 레이어
        _monsterLayer = LayerMask.NameToLayer("Monster");
        _bossLayer = LayerMask.NameToLayer("Boss");
    }

    // 탐색한 캐릭터
    private void OnTriggerStay2D(Collider2D collision)
    {
        // 탐색한 대상 플레이어 레이어 확인
        if (collision.gameObject.layer == _monsterLayer)
        {
            // 대상 정보
            MonsterController monster = collision.GetComponent<MonsterController>();

            // 탐색 대상에 몬스터 지정
            _controller._researchTarget = monster;

            return;
        }

        // 탐색한 대상 플레이어 레이어 확인
        if (collision.gameObject.layer == _bossLayer)
        {
            // 몬스터가 없거나 타겟이 몬스터가 아니면
            if (_controller._researchTarget == null || _controller._researchTarget.gameObject.layer != _monsterLayer)
            {
                // 대상 정보 확인
                MonsterController bossMonster = collision.GetComponent<MonsterController>();

                // 탐색 대상에 보스 몬스터 지정
                _controller._researchTarget = bossMonster;
            }
        }
    }

    // 벗어난 캐릭터
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 탐색 대상이 비어있지않고 나간 대상이 탐색 대상인 경우  
        if (_controller._researchTarget != null && collision.gameObject == _controller._researchTarget.gameObject)
        {
            // 탐색 대상 비우기
            _controller._researchTarget = null;
        }
    }
}
