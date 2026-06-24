using UnityEngine;
using System.Collections.Generic;


public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    public static QuestManager Instance
    { get { return instance; } }



    private List<Quest> questList=new List<Quest>();

    private List<Quest> completeList=new List<Quest>();




    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AcceptQuest(Quest _quest)
    {
        questList.Add(_quest);
        _quest.StartQuest();

        if (PlayerInventory.Instance != null && PlayerInventory.Instance.inventory != null)
        {
            int currentItemCount = PlayerInventory.Instance.inventory.HowManyItem(_quest.questData.goalItem);
            _quest.UpdateProgress(currentItemCount);
        }


        QuestUI.Instance.SaveQuestData(_quest);
    }

    public void CheckQuestProgress(ItemSO _item, int _currentCount)
    {
        foreach(Quest quest in questList)
        {
            if(quest.IsQuestProgress()&&quest.questData.goalItem==_item)
            {
                quest.UpdateProgress(_currentCount);
                if(!PlayerControl.Instance.IsInteracting)
                    QuestUI.Instance.UpdateTracker();
            }
        }
    }

    public void QuestComplete(Quest _quest)
    {
        questList.Remove(_quest);
        completeList.Add(_quest);
    }

    public Quest GetActiveQuest(QuestSO _questData)
    {
        foreach(Quest quest in questList)
        {
            if(quest.questData==_questData)
            {
                return quest;
            }
        }

        return null;
    }

    public List<Quest> GetQuestList()
    {
        return questList;
    }

    public bool IsQuestCompleted(QuestSO _questData)
    {
        foreach( Quest quest in completeList)
        {
            if(quest.questData==_questData)
                return true;
        }
        return false;
    }
}
