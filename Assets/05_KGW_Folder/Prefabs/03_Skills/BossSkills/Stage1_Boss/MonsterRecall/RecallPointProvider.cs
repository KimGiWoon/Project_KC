using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallPointProvider : MonoBehaviour
{
    [Header("Recall Point Setting")]
    [SerializeField] Transform[] _point;

    // 트랜스폼 전달을 위한 매개체
    public Transform[] _points => _point;
}
