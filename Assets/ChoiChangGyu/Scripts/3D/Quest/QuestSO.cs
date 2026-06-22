using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "Scriptable Objects/QuestSO")]
public class QuestSO : ScriptableObject
{
    public int questID;
    public string questName;
    public string questExplanation;
    public ItemSO goalItem;
    public int goalItemCount;
    public int RewardMoney;
}
