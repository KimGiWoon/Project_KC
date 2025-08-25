using UnityEngine;
using DG.Tweening;

public class NodeInteraction : MonoBehaviour
{
    // �ִϸ��̼� ���������� �ν����Ϳ��� ���ϰ� ������ �� �ֵ��� ������ ����ϴ�.
    [Header("Animation Settings")]
    public float punchScale = 0.2f;   // �󸶳� ũ�� ���� ���ΰ�
    public float duration = 0.5f;     // �ִϸ��̼� �ð� (��)
    public int vibrato = 10;          // ���� Ƚ��
    public float elasticity = 1f;     // ź�� (0~1 ����)

    // �� ��ũ��Ʈ�� ����� ���� ������Ʈ�� ���콺 Ŭ���� �����Ǹ� �ڵ����� ȣ��Ǵ� �Լ��Դϴ�.
    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + " ��尡 Ŭ���Ǿ����ϴ�!");

        // DoTween�� DOPunchScale �Լ��� ȣ���Ͽ� �ִϸ��̼��� �����մϴ�.
        // transform: �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ�� Transform ������Ʈ
        transform.DOPunchScale(
            new Vector3(punchScale, punchScale, 0), // x, y �������� punchScale ��ŭ ũ�⸦ Ű��ϴ�.
            duration,                               // �ִϸ��̼� ���� �ð�
            vibrato,                                // ���� Ƚ��
            elasticity                              // ź�� ����
        );
    }
}