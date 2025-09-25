using SDW;
using UnityEngine;

public class MoraleDeclineController : MonoBehaviour
{
    // 공격력 감소 계산 값 변환
    public float Init(float chance, float value)
    {
        float skillChance = chance * 0.01f;
        float attckDownValue = (Random.value < skillChance) ? value : 0f;

        // 사기 저하 사운드 플레이
        GameManager.Instance.Audio.Play2DSFX(AudioClipName.MeleePassiveSkill);

        Destroy(gameObject, 0.1f);

        return attckDownValue;
    }
}
