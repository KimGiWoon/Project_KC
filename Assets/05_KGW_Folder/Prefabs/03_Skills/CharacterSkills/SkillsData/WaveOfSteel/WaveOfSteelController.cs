using UnityEngine;

public class WaveOfSteelController : MonoBehaviour
{
    // 전체 공격력 계산 반환
    public float Init(float chance, float value, float damage)
    {
        float skillChance = chance * 0.01f;
        float skillValue = (Random.value < skillChance) ? value : 0f;
        Destroy(gameObject, 0.1f);

        return damage * skillValue;
    }
}
