using System.Collections;
using SDW;
using UnityEngine;
using TMPro;
using KSH;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("퀘스트 이름 텍스트")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("체크 이미지")]
    [SerializeField] private Image _checkImage;

    [HideInInspector] public DailyQuest dailyQuest;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        if (GameManager.Instance.Time != null)
            GameManager.Instance.Time.OnDailyReset += InitUI;

        if (GameManager.Instance.DailyQuest != null)
            GameManager.Instance.DailyQuest.OnQuestComplete += CheckUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.Time.OnDailyReset -= InitUI;

        GameManager.Instance.DailyQuest.OnQuestComplete -= CheckUI;
    }

    public void InitUI()
    {
        _checkImage.gameObject.SetActive(false);

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