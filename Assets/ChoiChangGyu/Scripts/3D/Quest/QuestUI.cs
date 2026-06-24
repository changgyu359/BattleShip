using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    private static QuestUI instance;
    public static QuestUI Instance
    { get { return instance; } }

    [SerializeField]
    private GameObject trackerPanel;
    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI progressText;


    private Quest currentQuest;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void ShutDownTrackerPanel()
    {
        trackerPanel.SetActive(false);
    }

    public void SaveQuestData(Quest _currentQuest)
    {
        currentQuest=_currentQuest;
    }

    public void UpdateTracker()
    {
        if(currentQuest==null)
        {
            List<Quest> questList = QuestManager.Instance.GetQuestList();

            if(questList.Count>0)
                currentQuest = questList[0];
            else 
            {
                trackerPanel.SetActive(false);
                return;
            }
        }

        
        trackerPanel.SetActive(true); 
        titleText.text="-"+currentQuest.questData.questName;
        progressText.text = "-" + $"{currentQuest.questData.goalItem.itemName}({currentQuest.CurrentAmount}/{currentQuest.questData.goalItemCount})";

        if (currentQuest.State==QuestState.CanComplete)
        {
            progressText.color = Color.green;
        }
        else if(currentQuest.State==QuestState.InProgress)
        {
            progressText.color = Color.white;
        }
    }
}
