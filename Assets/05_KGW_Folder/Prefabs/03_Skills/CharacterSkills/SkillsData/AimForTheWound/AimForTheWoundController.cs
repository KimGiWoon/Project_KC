using UnityEngine;

public class AimForTheWoundController : MonoBehaviour
{
    // 데미지 상승 계산 값 반환
    public float Init(float chance, float value, float damage)
    {
        float skillChance = chance * 0.01f;
        float skillValue = (Random.value < skillChance) ? value : 1f;
        Destroy(gameObject, 0.1f);

        return damage * skillValue;
    }
}
