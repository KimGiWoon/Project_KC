using SDW;
using UnityEngine;

public class RegenerativeStrikeContoller : MonoBehaviour
{
    public float Init(float chance, float value, float damage)
    {
        float skillChance = chance * 0.01f;
        float skillValue = (Random.value < skillChance) ? value : 0f;

        // 재생의 일격 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.HealerPassiveSkill);

        Destroy(gameObject, 0.1f);

        return damage * skillValue;
    }
}
