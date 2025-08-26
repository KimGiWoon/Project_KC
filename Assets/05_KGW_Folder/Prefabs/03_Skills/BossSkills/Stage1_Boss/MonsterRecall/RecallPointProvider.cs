using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallPointProvider : MonoBehaviour
{
    [Header("Recall Point Setting")]
    [SerializeField] Transform[] _point;
    public Transform[] _points => _point;
}
