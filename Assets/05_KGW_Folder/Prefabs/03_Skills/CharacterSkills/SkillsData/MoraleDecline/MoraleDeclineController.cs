using UnityEngine;

public class MoraleDeclineController : MonoBehaviour
{
    // 공격력 감소 계산 값 변환
    public float Init(float chance, float value)
    {
        float skillChance = chance * 0.01f;
        float attckDownValue = (Random.value < skillChance) ? value : 0f;
        Destroy(gameObject, 0.1f);

        return attckDownValue;
    }
}
