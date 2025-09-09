using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenerativeStrikeContoller : MonoBehaviour
{
    public float Init(float chance, float value, float damage)
    {
        float skillChance = chance * 0.01f;
        float skillValue = (Random.value < skillChance) ? value : 0f;
        Destroy(gameObject, 0.1f);

        return damage * skillValue;
    }
}
