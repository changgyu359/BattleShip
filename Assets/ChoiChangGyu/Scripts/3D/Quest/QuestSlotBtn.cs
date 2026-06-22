using TMPro;
using UnityEngine;

public class QuestSlotBtn : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questNameText;
    private QuestSO myQuestData;

    public void Setup(QuestSO _quest)
    {
        myQuestData = _quest;
        questNameText.text=myQuestData.questName;
    }

    public void OnClickThisSlot()
    {
        NPCQuestUI.Instance.SelectQuest(myQuestData);
    }
}
