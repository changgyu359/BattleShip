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
    }

    public void CheckQuestProgress(ItemSO _item, int _currentCount)
    {
        foreach(Quest quest in questList)
        {
            if(quest.IsQuestProgress()&&quest.questData.goalItem==_item)
                quest.UpdateProgress(_currentCount);
        }
    }

    public void QuestComplete(Quest _quest)
    {
        questList.Remove(_quest);
        completeList.Add(_quest);
    }
}
