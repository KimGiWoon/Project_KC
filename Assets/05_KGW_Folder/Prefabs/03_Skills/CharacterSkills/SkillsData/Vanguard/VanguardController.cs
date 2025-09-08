using UnityEngine;

public class VanguardController : MonoBehaviour
{
    // 방어력 상승값 반환
    public float Init(float value)
    {
        float armorUp = value;
        Destroy(gameObject, 0.1f);

        return armorUp;
    }
}
