using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace KSH
{
    public class GachaResultUI : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private GachaUI gachaPrefab;
        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private ScrollRect scrollRect;

        public void Show(List<CharacterData> characterDatas) //뽑은 캐릭터들을 보여주는 기능
        {
            foreach (Transform child in content) //content안에 있는 이전 뽑기 결과들
            {
                Destroy(child.gameObject); //모두 삭제
            }
        
            scrollRect.verticalNormalizedPosition = 1f; //스크롤 위치 초기화
        
            List<RectTransform> spawnedRects = new List<RectTransform>(); //RectTransform 리스트 생성

            float startY = -1000; //애니메이션 시작 위치
            float spacing = 250; //캐릭터UI 간 간격

            for (int i = 0; i < characterDatas.Count; i++) //뽑힌 캐릭터 수 만큼 생성
            {
                var gacha = Instantiate(gachaPrefab, content); //뽑힌 캐릭터 UI을 content안에 생성
                gacha.SetData(characterDatas[i]); //캐릭터 데이터 적용

                RectTransform rect = gacha.GetComponent<RectTransform>();
                spawnedRects.Add(rect); //RectTransform을 만든 리스트에 저장
            
                if( i == 0) //만약 인덱스가 0이라면
                    rect.anchoredPosition = new Vector2(x, startY);
                else    
                    rect.anchoredPosition = new Vector2(x, startY * i);
            }
        
            for (int i = 0; i < spawnedRects.Count; i++) //생성된 UI 모두 적용
            {
                RectTransform rect = spawnedRects[i]; //리스트 안에 i번째 요소 가져오기
                Vector2 finalPos = new Vector2(x, y - i * spacing); //마지막 위치

                rect.DOAnchorPos(finalPos, 0.5f) //startpos에서 finalPos까지 0.5초간 이동
                    .SetEase(Ease.OutBack,0.8f) //튕기는 효과
                    .SetDelay(0.2f * i); //순차적으로 등장
            }
        }
    }
}