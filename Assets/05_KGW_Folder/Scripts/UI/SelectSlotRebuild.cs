using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSlotRebuild : MonoBehaviour
{
    private void Start()
    {
        // 슬롯이 다시 생성될 위치 전달
        CharacterSelectManager.Instance.SelectSlotReset(transform);
    }
}
