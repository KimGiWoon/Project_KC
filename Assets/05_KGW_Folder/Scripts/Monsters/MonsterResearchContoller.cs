using UnityEngine;

public class MonsterResearchContoller : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] MonsterController _controller;

    // 탐색한 캐릭터
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_controller._researchTarget != null) return;

        // 탐색한 대상 플레이어 레이어 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 대상 정보
            CharacterController character = collision.GetComponent<CharacterController>();

            // 탐색 대상에 지정
            _controller._researchTarget = character;
        }
    }
}
