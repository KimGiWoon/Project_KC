using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{   
    [Header("Camera Setting Reference")]
    [SerializeField] float _followSpeed;
    [SerializeField] public bool _isDelayMove;

    Transform _followTarget;
    Vector3 _offset = new Vector3(0, 2f, -10f);


    // �÷��̾ �����̰� �� �� ī�޶� �̵�
    private void LateUpdate()
    {
        FollowCamera();
    }

    // Ÿ�� ����
    public void SetTarget(Transform target)
    {
        _followTarget = target;
    }

    // Ÿ�� ���󰡱�
    private void FollowCamera()
    {
        if (_followTarget == null)
        {
            return;
        }

        // ī�޶� ��ġ ����
        Vector3 camPos = _followTarget.position + _offset;
        // ī�޶� ��ġ �̵�, ������ �̵� ��� On
        if (_isDelayMove)
        {
            Vector3 camMovePos = Vector3.Lerp(transform.position, camPos, _followSpeed * Time.deltaTime);
            transform.position = camMovePos;
        }
        else // �ﰢ �̵� ��� On
        {
            transform.position = camPos;
        }
    }

}
