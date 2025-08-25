using UnityEngine;

public class CharacterResearchContoller : MonoBehaviour
{
    [Header("Controller Reference")]
    [SerializeField] CharacterController _controller;

    // 탐색한 캐릭터
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_controller._researchTarget != null) return;

        // 탐색한 대상 플레이어 레이어 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            // 대상 정보
            MonsterController monster = collision.GetComponent<MonsterController>();

            // 탐색 대상에 지정
            _controller._researchTarget = monster;
        }
    }
}
