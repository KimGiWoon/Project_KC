using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGacha : MonoBehaviour
{
    [SerializeField] private CharacterGacha gacha;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gacha.GetRandomCharacter(); // 스페이스바 누르면 가챠 실행
        }
    }
}
