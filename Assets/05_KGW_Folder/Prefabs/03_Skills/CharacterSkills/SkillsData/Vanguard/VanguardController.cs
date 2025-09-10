using System.Collections;
using UnityEngine;

public class VanguardController : MonoBehaviour
{
    // 피해 감소 상승 반환
    public float Init(float value)
    {
        float _reductionUp = value;

        Destroy(gameObject, 0.1f);

        return _reductionUp;
    }
}
