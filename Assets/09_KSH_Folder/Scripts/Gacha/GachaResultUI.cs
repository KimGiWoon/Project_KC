using System;
using System.Collections.Generic;
using DG.Tweening;
using SDW;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KSH
{
    public class GachaResultUI : BaseUI
    {
        [Header("Gacha Type")]
        [SerializeField] private GameObject _singlePanel;
        [SerializeField] private GameObject _tenPanel;

        [Header("Single Type Components")]
        [SerializeField] private Image _gachaCharacterImage;
        [SerializeField] private Image _rareImage;
        [SerializeField] private Image _normalImage;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private TextMeshProUGUI _gachaGainText;
        [SerializeField] private GameObject _sugarStartObject;
        [SerializeField] private GameObject _beadsObject;
        [SerializeField] private GachaUI singleUI;

        [Header("Ten Type Components")]
        [SerializeField] private Transform content;
        [SerializeField] private GachaUI gachaPrefab;
        [SerializeField] private int x;
        [SerializeField] private int y;

        [Header("ETC Components")]
        [SerializeField] private CharacterGacha _gacha;
        [SerializeField] private Button _backButton;
        private bool _isSingle;

        public Action<UIName> OnUIOpenRequested;
        public Action<UIName> OnUICloseRequested;

        private void Awake()
        {
            _panelContainer.SetActive(false);
            _gacha = GameManager.Instance.Gacha;
            _gacha.SetGachaResultUI(this);
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
            _backButton.interactable = false;
            _backButton.gameObject.SetActive(false);
            OnUIOpenRequested?.Invoke(UIName.GachaMainUI);
            OnUICloseRequested?.Invoke(UIName.GachaResultUI);
        }

        public override void Open()
        {
            base.Open();
            _backButton.interactable = true;
            _backButton.gameObject.SetActive(true);

            var gachaResult = _gacha.GetGacha();

            if (_isSingle) ShowSingleGacha(gachaResult);
            else ShowTenGacha(gachaResult);
        }

        public override void Close()
        {
            base.Close();
            _singlePanel.SetActive(false);
            _tenPanel.SetActive(false);
        }

        private void ShowSingleGacha(ResultData characterData)
        {
            var data = characterData.Result[0];

            bool isFirst = characterData.CurrentBead[0] == 0;

            singleUI.SetData(
                data,
                characterData.GainedStarCandy[0],
                characterData.GainedBead[0],
                characterData.CurrentBead[0],       
                isFirst,
                PullType.One
                );

            _singlePanel.SetActive(true);
        }

        // public void Show(List<CharacterData> characterDatas) //뽑은 캐릭터들을 보여주는 기능
        private void ShowTenGacha(ResultData characterDatas) //뽑은 캐릭터들을 보여주는 기능
        {
            foreach (Transform child in content) //content안에 있는 이전 뽑기 결과들
            {
                Destroy(child.gameObject); //모두 삭제
            }

            var spawnedRects = new List<RectTransform>(); //RectTransform 리스트 생성

            float startY = -1000; //애니메이션 시작 위치
            float spacing = 250; //캐릭터UI 간 간격

            for (int i = 0; i < characterDatas.Result.Count; i++) //뽑힌 캐릭터 수 만큼 생성
            {
                var gacha = Instantiate(gachaPrefab, content); //뽑힌 캐릭터 UI을 content안에 생성

                bool isFirst = characterDatas.CurrentBead[i] == 0;

                gacha.SetData(
                    characterDatas.Result[i],
                    characterDatas.GainedStarCandy[i],
                    characterDatas.GainedBead[i],
                    characterDatas.CurrentBead[i],
                    // _reward.ownedCharacters[characterDatas.Result[i]._chaBaseData.ChaName]
                    isFirst,
                    PullType.Ten
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

            _tenPanel.SetActive(true);
        }

        public void SetGachaType(bool isSingle) => _isSingle = isSingle;
    }
}