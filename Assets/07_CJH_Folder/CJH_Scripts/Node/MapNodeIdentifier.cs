using System.Collections.Generic;
using UnityEngine;

// �� ������Ʈ�� ���ø� ������ ���� ��� ��ġ�� �ĺ��ϴ� ���Ҹ� �մϴ�.
public class MapNodeIdentifier : MonoBehaviour
{
    // �ν����Ϳ��� ���� ������ ��
    public int floorIndex; // �� ��° ���� ���ϴ°� (0���� ����)
    public int nodeIndexInFloor; // �ش� ������ �� ��° ����ΰ� (0���� ����)

    public List<MapNodeIdentifier> connections;
}