using SDW;
using UnityEngine;
using TMPro;
using KSH;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("퀘스트 이름 텍스트")]
    private TextMeshProUGUI nameText;

    [Header("체크 이미지")]
    private Image _checkImage;

    [HideInInspector] public DailyQuest dailyQuest;

    private void Awake()
    {
        nameText = GetComponentInChildren<TextMeshProUGUI>(true);
        var checkImages = GetComponentsInChildren<Image>(true);
        _checkImage = checkImages[2];
        _checkImage.gameObject.SetActive(false);
    }

    public void Start()
    {
        if (GameManager.Instance.Time != null)
            GameManager.Instance.Time.OnDailyReset += InitUI;

        if (GameManager.Instance.DailyQuest != null)
            GameManager.Instance.DailyQuest.OnQuestComplete += CheckUI;
    }

    public void InitUI()
    {
        if (dailyQuest == null) return;

        if (nameText != null)
            nameText.text = dailyQuest.questName;
        _checkImage.gameObject.SetActive(dailyQuest.isComplete);
    }

    public void CheckUI()
    {
        if (_checkImage.gameObject != null && dailyQuest != null)
            _checkImage.gameObject.SetActive(dailyQuest.isComplete);
    }
}