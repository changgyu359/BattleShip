using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCQuestUI : MonoBehaviour
{
    private static NPCQuestUI instance;
    public static NPCQuestUI Instance
    { get { return instance; } }

    [Header("퀘스트 상세와 버튼")]
    [SerializeField] private GameObject questUIPanel;
    //[SerializeField] private GameObject acceptBtn;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescText;
    [SerializeField] private TextMeshProUGUI questRewardText;

    [Header("퀘스트리스트")]
    [SerializeField] private Transform contentParent;
    [SerializeField] GameObject questSlotPrefab;

    private QuestSO currentSelectedQuest;

    private List<QuestSlotBtn> slotList=new List<QuestSlotBtn>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void OpenQuestUI(List<QuestSO> _questsToOffer)
    {
        questUIPanel.SetActive(true);

        if (_questsToOffer == null || _questsToOffer.Count == 0)
        {
            
            return;
        }

        foreach (QuestSlotBtn slot in slotList)
            slot.gameObject.SetActive(false);

        foreach (QuestSO _quest in _questsToOffer)
        {
            
            foreach (QuestSlotBtn slot in slotList)
            {
                Debug.Log("이거 함??");
                if (!slot.gameObject.activeSelf)
                {
                    slot.gameObject.SetActive(true);
                    slot.Setup(_quest);
                }
            }
            GameObject newBtnObj = Instantiate(questSlotPrefab, contentParent);


            QuestSlotBtn slotBtn = newBtnObj.GetComponent<QuestSlotBtn>();
            slotBtn.Setup(_quest);
            slotList.Add(slotBtn);
        }

        
        SelectQuest(_questsToOffer[0]);
    }


    public void SelectQuest(QuestSO _quest)
    {
        currentSelectedQuest = _quest;

        questTitleText.text=currentSelectedQuest.questName;
        questDescText.text=currentSelectedQuest.questExplanation;
        questRewardText.text = "보상:" + currentSelectedQuest.RewardMoney + "$";

        //acceptBtn.SetActive(true);
    }





}
