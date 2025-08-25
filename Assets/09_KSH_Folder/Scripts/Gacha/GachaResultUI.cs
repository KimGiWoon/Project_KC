using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaResultUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GachaUI gachaPrefab;

    public void Show(List<CharacterData> characterDatas)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var c in characterDatas)
        {
            var gacha = Instantiate(gachaPrefab, content);
            gacha.SetData(c);
        }
    }
}
