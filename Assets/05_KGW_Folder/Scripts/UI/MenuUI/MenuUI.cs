using System;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : BaseUI
{
    [Header("Battle UI Reference")]
    [SerializeField] private BattleUI _battleUI;

    [Header("UI Container")]
    [SerializeField] private GameObject _isOkayContainer;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    private Slider _slider_BGM; // BGM 슬라이더
    private Slider _slider_SFX; // SFX 슬라이더
    [SerializeField] private Button _continueButton; // 계속하기 버튼
    [SerializeField] private Button _lobbyButton; // 로비 이동 버튼

    [SerializeField] private GameObject _popupBackground;

    private RectTransform _panelRect;

    public Action<UIName> OnUIOpenRequested;
    public Action<UIName> OnUICloseRequested;

    private void Awake()
    {
        _panelContainer.SetActive(false);
        _panelRect = _panelContainer.GetComponent<RectTransform>();
        _slider_BGM = _panelContainer.GetComponentInChildren<Slider>();
        _slider_SFX = _panelContainer.GetComponentInChildren<Slider>();

        _isOkayContainer.SetActive(false);
    }

    private void OnEnable()
    {
        // 버튼, 슬라이드 등록
        //_slider_BGM.onValueChanged.AddListener()
        //_slider_SFX.onValueChanged.AddListener()

        _confirmButton.onClick.AddListener(ConfirmButtonClicked);
        _cancelButton.onClick.AddListener(CancelButtonClicked);
        _continueButton.onClick.AddListener(ContinueButtonClick);
        _lobbyButton.onClick.AddListener(LobbyButtonClick);
    }

    private void OnDisable()
    {
        _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        _cancelButton.onClick.RemoveListener(CancelButtonClicked);
        _continueButton.onClick.RemoveListener(ContinueButtonClick);
        _lobbyButton.onClick.RemoveListener(LobbyButtonClick);
    }

    protected override void Start()
    {
        base.Start();
        // 사운드 볼륨 초기화
        SoundVolumeInit();
    }

    private void Update()
    {
        if (!_panelContainer.activeSelf) return;

        //# 안드로이드 터치 감지
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var touchPos = Input.GetTouch(0).position;

            //# 패널 안에 터치가 있는지 확인
            if (!RectTransformUtility.RectangleContainsScreenPoint(_panelRect, touchPos))
            {
                OnUICloseRequested?.Invoke(UIName.MenuUI);
                _popupBackground.SetActive(false);
            }
        }
    }

    // 사운드 볼륨 초기화
    private void SoundVolumeInit()
    {
        //_slider_BGM.value = ;
        //_slider_SFX.value = ;
    }

    // 계속하기 버튼 클릭
    private void ContinueButtonClick()
    {
        // TODO : 김기운 : 추후에 UI매니저에서 관리
        // 메뉴 패널 비활성화
        OnUICloseRequested?.Invoke(UIName.MenuUI);
        _popupBackground.SetActive(false);

        _battleUI._isOnMenu = false;
        //todo 기획팀에 문의해야 함
        //todo UI가 닫혔을 때 게임이 다시 시작되도록
    }

    // 로비 이동 버튼 클릭
    private void LobbyButtonClick() => _isOkayContainer.SetActive(true);

    private void ConfirmButtonClicked()
    {
        OnUIOpenRequested?.Invoke(UIName.DefeatChapterUI);
        OnUICloseRequested?.Invoke(UIName.MenuUI);
        _isOkayContainer.SetActive(false);
    }
    private void CancelButtonClicked() => _isOkayContainer.SetActive(false);
}