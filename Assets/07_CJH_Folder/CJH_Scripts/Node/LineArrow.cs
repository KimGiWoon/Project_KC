using UnityEngine;
using System.Collections;

public class LineArrow : MonoBehaviour
{
    public float blinkSpeed = 0.5f; // 깜빡이는 속도
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}