using UnityEngine;
using System;

public enum QuestState
{
    NotStarted,
    InProgress,
    CanComplete,
    Compleleted

}
[Serializable]
public class Quest
{
    
    public QuestSO questData { get; private set; }

    public QuestState State { get; private set; }

    public int CurrentAmount {  get; private set; }


    public Quest(QuestSO data)
    {
        questData = data;
        State = QuestState.NotStarted;
        CurrentAmount = 0;
    }

    public void StartQuest()
    {
        if (State == QuestState.NotStarted)
            State=QuestState.InProgress;
    }

    public void UpdateProgress(int _invenCount)
    {
        if (State != QuestState.InProgress && State != QuestState.CanComplete) return;

        CurrentAmount = _invenCount;

        if (CurrentAmount >= questData.goalItemCount)
            State = QuestState.CanComplete;
        else
            State = QuestState.InProgress;

    }

    public void CompleteQuest()
    {
        if (State == QuestState.CanComplete)
            State = QuestState.Compleleted;
    }

    public bool IsQuestProgress()
    {
        return (State == QuestState.InProgress||State==QuestState.CanComplete);
    }


}
