using System;
using System.Collections.Generic;
using DG.Tweening;
using SDW;
using UnityEngine;
using UnityEngine.UI;

namespace KSH
{
    public class GachaResultUI : BaseUI
    {
        [SerializeField] private Transform content;
        [SerializeField] private GachaUI gachaPrefab;
        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private CharacterGacha _gacha;
        [SerializeField] private Button _backButton;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;
        private Dictionary<string, bool> _isFirstCharacter = new Dictionary<string, bool>();

        private void Awake()
        {
            _panelContainer.SetActive(false);
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(BackButtonClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(BackButtonClicked);
        }

        private void BackButtonClicked()
        {
            OnUIOpenRequested?.Invoke(UIName.GachaMainUI);
            OnUICloseRequested?.Invoke(UIName.GachaResultUI);
        }

        public override void Open()
        {
            base.Open();
            Show(_gacha.GetGacha());
        }

        // public void Show(List<CharacterData> characterDatas) //뽑은 캐릭터들을 보여주는 기능
        public void Show(ResultData characterDatas) //뽑은 캐릭터들을 보여주는 기능
        {
            foreach (Transform child in content) //content안에 있는 이전 뽑기 결과들
            {
                Destroy(child.gameObject); //모두 삭제
            }

            scrollRect.verticalNormalizedPosition = 1f; //스크롤 위치 초기화

            var spawnedRects = new List<RectTransform>(); //RectTransform 리스트 생성

            float startY = -1000; //애니메이션 시작 위치
            float spacing = 250; //캐릭터UI 간 간격

            for (int i = 0; i < characterDatas.Result.Count; i++) //뽑힌 캐릭터 수 만큼 생성
            {
                var gacha = Instantiate(gachaPrefab, content); //뽑힌 캐릭터 UI을 content안에 생성
                // gacha.SetData(characterDatas[i]); //캐릭터 데이터 적용

                if (!_isFirstCharacter.ContainsKey(characterDatas.Result[i].characterName))
                    _isFirstCharacter[characterDatas.Result[i].characterName] = true;
                else
                    _isFirstCharacter[characterDatas.Result[i].characterName] = false;
                gacha.SetData(
                    characterDatas.Result[i],
                    characterDatas.GainedStarCandy[i],
                    characterDatas.GainedBead[i],
                    characterDatas.CurrentBead[i],
                    _isFirstCharacter[characterDatas.Result[i].characterName]
                ); //캐릭터 데이터 적용

                var rect = gacha.GetComponent<RectTransform>();
                spawnedRects.Add(rect); //RectTransform을 만든 리스트에 저장

                if (i == 0) //만약 인덱스가 0이라면
                    rect.anchoredPosition = new Vector2(x, startY);
                else
                    rect.anchoredPosition = new Vector2(x, startY * i);
            }

            for (int i = 0; i < spawnedRects.Count; i++) //생성된 UI 모두 적용
            {
                var rect = spawnedRects[i]; //리스트 안에 i번째 요소 가져오기
                var finalPos = new Vector2(x, y - i * spacing); //마지막 위치

                rect.DOAnchorPos(finalPos, 0.5f) //startpos에서 finalPos까지 0.5초간 이동
                    .SetEase(Ease.OutBack, 0.8f) //튕기는 효과
                    .SetDelay(0.2f * i); //순차적으로 등장
            }
        }
    }
}