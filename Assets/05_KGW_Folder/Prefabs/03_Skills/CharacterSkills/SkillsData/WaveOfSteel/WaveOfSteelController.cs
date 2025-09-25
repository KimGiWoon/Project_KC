using SDW;
using UnityEngine;

public class WaveOfSteelController : MonoBehaviour
{
    // 전체 공격력 계산 반환
    public float Init(float chance, float value, float damage)
    {
        float skillChance = chance * 0.01f;
        float skillValue = (Random.value < skillChance) ? value : 0f;

        // 강철의 파동 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.RangedPassiveSkill);

        Destroy(gameObject, 0.1f);

        return damage * skillValue;
    }
}
