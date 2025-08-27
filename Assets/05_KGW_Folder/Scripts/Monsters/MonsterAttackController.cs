using UnityEngine;

public class MonsterAttackController : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] MonsterController _controller;

    // 사거리에 들어온 캐릭터
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CharacterController character = collision.GetComponent<CharacterController>();

            // 공격 가능한 캐릭터에 해당 캐릭터가 없으면
            if (!_controller._attackTargets.Contains(character))
            {
                // 감지된 캐릭터 추가
                _controller._attackTargets.Add(character);

                // 현재 공격 대상이 없으면
                if (_controller._attackTarget == null)
                {
                    // 감지된 캐릭터를 공격 대상에 지정
                    _controller._attackTarget = character;
                }
            }
        }
    }

    // 사거리에서 벗어난 캐릭터
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CharacterController character = collision.GetComponent<CharacterController>();

            // 공격 가능한 캐릭터에 해당 캐릭터가 있으면 
            if (_controller._attackTargets.Contains(character))
            {
                // 캐릭터 삭제
                _controller._attackTargets.Remove(character);
            }

            // 현재의 타깃이 사거리에서 벗어나면
            if (_controller._attackTarget == character)
            {
                // 다음 순서의 캐릭터가 있으면 현재 공격 대상으로 변경
                _controller._attackTarget = _controller._attackTargets.Count > 0 ? _controller._attackTargets[0] : null;
            }
        }
    }
}
