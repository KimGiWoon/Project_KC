using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class EffectIconUI : MonoBehaviour
{
    public enum IconBehavior
    {
        DurationBased,      // 지속시간 동안 유지되다 사라짐 (버프/디버프)
        InstantFadeOut,     // 생성되자마자 사라짐 (회복/부활)
        Barrier             // 특정 조건(피격)에 의해 상태가 변하고 사라짐
    }

    [SerializeField] private Image iconImage;
    // [SerializeField] private TextMeshProUGUI timerText; // 버프/디버프 외에는 타이머가 불필요하므로 선택사항

    private CanvasGroup canvasGroup;
    private Coroutine effectCoroutine;
    private Action onComplete;

    public IconBehavior Behavior { get; private set; }
    private bool isBarrierBroken = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 아이콘을 초기화하고 지정된 행동 타입에 따라 동작을 시작합니다.
    /// </summary>
    public void Initialize(IconBehavior behavior, Sprite initialSprite, float duration, Action onCompleteCallback)
    {
        this.Behavior = behavior;
        iconImage.sprite = initialSprite;
        this.onComplete = onCompleteCallback;

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        switch (behavior)
        {
            case IconBehavior.DurationBased:
                effectCoroutine = StartCoroutine(DurationLifecycle(duration));
                break;
            case IconBehavior.InstantFadeOut:
                effectCoroutine = StartCoroutine(InstantFadeOutLifecycle());
                break;
            case IconBehavior.Barrier:
                // 보호막은 외부 트리거(OnBarrierBreak)가 있을 때까지 대기
                canvasGroup.alpha = 1f;
                break;
        }
    }

    /// <summary>
    /// 지속시간이 있는 효과를 새 효과로 갱신합니다. (버프/디버프용)
    /// </summary>
    public void Refresh(Sprite sprite, float duration)
    {
        if (Behavior != IconBehavior.DurationBased) return;

        iconImage.sprite = sprite;
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(DurationLifecycle(duration));
    }

    // 1. 지속시간 기반 라이프사이클 (버프/디버프)
    private IEnumerator DurationLifecycle(float duration)
    {
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeOut(0.5f));
        CompleteAndDestroy();
    }

    // 2. 즉시 소멸 라이프사이클 (회복/부활)
    private IEnumerator InstantFadeOutLifecycle()
    {
        // 부활의 경우, 생성 후 잠시 대기했다가 Fade Out
        // 필요하다면 여기에 Fade In 로직 추가 가능
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.1f); // 생성 확인을 위한 짧은 딜레이
        yield return StartCoroutine(FadeOut(2.0f)); // 기획서의 '2초동안 페이드아웃' 반영
        CompleteAndDestroy();
    }

    /// <summary>
    /// 보호막이 깨졌을 때 호출되는 외부 함수
    /// </summary>
    public void OnBarrierBreak(Sprite brokenSprite)
    {
        if (Behavior != IconBehavior.Barrier || isBarrierBroken) return;
        isBarrierBroken = true;

        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(BarrierBreakLifecycle(brokenSprite));
    }

    // 3. 보호막 파괴 라이프사이클
    private IEnumerator BarrierBreakLifecycle(Sprite brokenSprite)
    {
        iconImage.sprite = brokenSprite;
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1.0f); // 깨진 이미지 1초 유지
        yield return StartCoroutine(FadeOut(0.5f));
        CompleteAndDestroy();
    }

    private IEnumerator FadeOut(float fadeDuration)
    {
        float counter = 0f;
        float startAlpha = canvasGroup.alpha;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, counter / fadeDuration);
            yield return null;
        }
    }

    private void CompleteAndDestroy()
    {
        onComplete?.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// 캐릭터 사망 등 외부 요인으로 즉시 효과를 제거할 때 호출합니다.
    /// </summary>
    public void ForceRemove()
    {
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        CompleteAndDestroy();
    }
}