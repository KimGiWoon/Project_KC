using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JJY;

public class EffectController : MonoBehaviour
{
    // 이펙트 타입과 게임 오브젝트를 자동으로 연결해서 담아둘 딕셔너리
    private Dictionary<EffectType, GameObject> effectObjects = new Dictionary<EffectType, GameObject>();

    void Awake()
    {
        foreach (Transform categoryParent in transform) // Buff, Debuff, Instant 등 부모들을 순회
        {
            foreach (Transform effect in categoryParent) // 각 부모 아래의 실제 이펙트들을 순회
            {
                // 이름으로 Enum 값을 찾아서 딕셔너리에 추가
                if (System.Enum.TryParse(effect.name, out EffectType type))
                {
                    effectObjects[type] = effect.gameObject;
                    effect.gameObject.SetActive(false); // 게임 시작 시 모든 이펙트 비활성화
                    Debug.Log($"<color=green>[{transform.root.name}] 이펙트 등록 성공: {type.ToString()}</color>");
                }
                else
                {
                    Debug.LogWarning($"[{transform.root.name}] '{effect.name}'을 EffectType으로 변환 실패. 프리팹 오브젝트 이름을 확인해주세요.");
                }
            }
        }
    }

    /// <summary>
    /// 외부(BuffManager)에서 이펙트를 켜달라고 요청하는 함수
    /// </summary>
    /// <param name="type">보여줄 이펙트 종류</param>
    /// <param name="duration">지속 시간 (0이면 계속 켜짐, 0보다 크면 해당 시간 후 자동 비활성화)</param>
    public void ShowEffect(EffectType type, float duration)
    {
        Debug.Log($"<color=yellow>[{transform.root.name}] ShowEffect 호출됨 => 타입: {type}, 지속시간: {duration}</color>");

        if (effectObjects.TryGetValue(type, out GameObject effectObject))
        {
            if (duration > 0)
            {
                // 이미 켜져 있는 코루틴이 있다면 중지하고 새로 시작 (효과 시간 갱신)
                StopCoroutine(nameof(EffectCoroutine));
                StartCoroutine(EffectCoroutine(effectObject, duration));
            }
            else
            {
                effectObject.SetActive(true); // Barrier처럼 계속 켜져 있어야 하는 효과
            }
        }
        else
        {
            Debug.LogWarning($"[{transform.root.name}] '{type}' 타입의 이펙트가 딕셔너리에 등록되어 있지 않습니다.");
        }
    }

    /// <summary>
    /// 외부(BuffManager)에서 지속형 이펙트를 꺼달라고 요청하는 함수
    /// </summary>
    public void HideEffect(EffectType type)
    {
        if (effectObjects.TryGetValue(type, out GameObject effectObject))
        {
            effectObject.SetActive(false);
        }
    }

    // 지정된 시간 후에 이펙트를 비활성화하는 코루틴
    private IEnumerator EffectCoroutine(GameObject effectObject, float duration)
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        effectObject.SetActive(false);
    }
}