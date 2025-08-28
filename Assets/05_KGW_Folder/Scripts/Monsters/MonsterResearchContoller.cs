using UnityEngine;

public class MonsterResearchContoller : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] MonsterController _controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 탐색한 대상 레이어 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 적이 감지되고 보스몬스터이면 스킬 사용
            if (_controller.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                // 적 감지
                _controller._isDetect = true;

                if (!_controller._isFirst)
                {
                    _controller.UseSkill();
                }
            }
        }
    }

    // 탐색한 캐릭터
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_controller._researchTarget != null) return;

        // 탐색한 대상 레이어 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 대상 정보
            MyCharacterController character = collision.GetComponent<MyCharacterController>();

            // 탐색 대상에 지정
            _controller._researchTarget = character;
        }
    }
}
