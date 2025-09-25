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
    
    [SerializeField] private TextMeshProUGUI countText;

    [HideInInspector] public DailyQuest dailyQuest;

    private void OnEnable()
    {
        if (GameManager.Instance.Time != null)
            GameManager.Instance.Time.OnDailyReset += InitUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.Time.OnDailyReset -= InitUI;
    }

    public void InitUI()
    {
        _checkImage.gameObject.SetActive(false);

        if (dailyQuest == null) return;

        if (nameText != null)
            nameText.text = dailyQuest.questName;
        
        if(_checkImage != null)
            _checkImage.gameObject.SetActive(dailyQuest.isComplete);
      
        UpdateCountText(dailyQuest);
    }

    public void CheckUI()
    {
        if (_checkImage.gameObject != null && dailyQuest != null)
            _checkImage.gameObject.SetActive(dailyQuest.isComplete);
    }

    public void UpdateCountText(DailyQuest dailyQuest)
    {
        if (countText == null) return;
        countText.text = $"{dailyQuest.currentProgress}/{dailyQuest.questGoal}";
    }
}