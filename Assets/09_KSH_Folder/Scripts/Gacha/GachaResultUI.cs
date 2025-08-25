using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GachaUI gachaPrefab;
    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private ScrollRect scrollRect;

    public void Show(List<CharacterData> characterDatas)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        
        scrollRect.verticalNormalizedPosition = 1f; //스크롤 위치 초기화
        
        List<RectTransform> spawnedRects = new List<RectTransform>();

        float startY = -1000;
        float spacing = 250;          

        for (int i = 0; i < characterDatas.Count; i++)
        {
            var gacha = Instantiate(gachaPrefab, content);
            gacha.SetData(characterDatas[i]);

            RectTransform rect = gacha.GetComponent<RectTransform>();
            spawnedRects.Add(rect);
            
            if( i == 0)
                rect.anchoredPosition = new Vector2(x, startY);
            else    
                rect.anchoredPosition = new Vector2(x, startY * i);
        }
        
        for (int i = 0; i < spawnedRects.Count; i++)
        {
            RectTransform rect = spawnedRects[i];
            Vector2 finalPos = new Vector2(x, y - i * spacing);

            rect.DOAnchorPos(finalPos, 0.5f)
                .SetEase(Ease.OutBack,0.8f)
                .SetDelay(0.2f * i);
        }
    }
}
