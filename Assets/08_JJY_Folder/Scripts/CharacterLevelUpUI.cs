using System.Collections.Generic;
using JJY;
using SDW;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLevelUpUI : BaseUI
{
    [Header("UI Components")]
    // 캐릭터 레벨업 씬 입장 버튼
    [SerializeField] private Button _toCharacterLevelUpButton;
    [SerializeField] private Button _useItemButton;

    [Header("Content Transform")]
    [SerializeField] private GameObject _contents;

    private TweenAnimation _tweenAnimation;
    private CharacterLevelUpUIManager _characterLevelUpUIManager;

    private void Awake()
    {
        // _panelContainer.SetActive(false);
        _tweenAnimation = GetComponent<TweenAnimation>();
    }
    protected override void Start()
    {
        base.Start();
        // _characterLevelUpUIManager = CharacterLevelUpUIManager.Instance;
    }
    private void OnEnable()
    {
        _toCharacterLevelUpButton.onClick.AddListener(ToCharacterLevelUpButtonClicked);
        _useItemButton.onClick.AddListener(UseItemButtonClicked);
    }
    private void OnDisable()
    {
        _toCharacterLevelUpButton.onClick.RemoveListener(ToCharacterLevelUpButtonClicked);
        _useItemButton.onClick.RemoveListener(UseItemButtonClicked);

    }

    private void ToCharacterLevelUpButtonClicked()
    {
        _characterLevelUpUIManager.InitCharacterList();
    }
    private void UseItemButtonClicked()
    {
        _characterLevelUpUIManager.UseItem();
    }
    public void SetContent(List<GameObject> characterList)
    {
        foreach (var character in characterList)
        {
            character.transform.SetParent(_contents.transform, false);
            character.gameObject.SetActive(true);
        }
    }
}
