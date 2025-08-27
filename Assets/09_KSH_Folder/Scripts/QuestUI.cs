using SDW;
using UnityEngine;
using TMPro;
using KSH;

public class QuestUI : MonoBehaviour
{
    [Header("퀘스트 이름 텍스트")]
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("체크 이미지")]
    [SerializeField] private GameObject checkImage;

    [HideInInspector] public DailyQuest dailyQuest;

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
        checkImage.SetActive(false);
    }

    public void CheckUI()
    {
        if (checkImage != null && dailyQuest != null)
            checkImage.SetActive(dailyQuest.isComplete);
    }
}