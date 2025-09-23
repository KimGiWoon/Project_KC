using UnityEngine;

/// <summary>
/// 캐릭터별 UI 부모 Transform을 담는 데이터 컨테이너 클래스입니다.
/// 캐릭터 프리팹의 루트에 추가하고, 자식으로 만든 위치 오브젝트들을 연결합니다.
/// </summary>
public class CharacterUIParents : MonoBehaviour
{
    [Header("UI 아이콘 생성 위치")]
    public Transform buffParent;      // 우상단 (버프)
    public Transform debuffParent;    // 좌상단 (디버프)
    public Transform instantParent;   // 하단 (회복, 부활 등 즉시효과)
    public Transform barrierParent;   // 좌측 (보호막)
}