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
    [SerializeField] private GameObject acceptBtn;
    [SerializeField] private GameObject completeBtn;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescText;
    [SerializeField] private TextMeshProUGUI questRewardText;
    [SerializeField] private TextMeshProUGUI questStateText;

    [Header("퀘스트리스트")]
    [SerializeField] private Transform contentParent;
    [SerializeField] GameObject questSlotPrefab;


    [SerializeField]
    private TextMeshProUGUI marketTalk;

    private List<QuestSO> currentOfferList;

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
        currentOfferList = _questsToOffer;

        questUIPanel.SetActive(true);

        List<QuestSO> availableQuests=new List<QuestSO>();


        if(_questsToOffer!=null)
        {
            foreach(QuestSO quest in _questsToOffer)
            {
                if(!QuestManager.Instance.IsQuestCompleted(quest))
                    availableQuests.Add(quest);
            }
        }

        if (availableQuests.Count == 0)
        {
            foreach(QuestSlotBtn slot in slotList)
                slot.gameObject.SetActive(false);

            ThereIsNoQuest();
            return;
        }

        foreach (QuestSlotBtn slot in slotList)
            slot.gameObject.SetActive(false);

        
        for(int i = 0;i<availableQuests.Count;i++)
        {
            if(i<slotList.Count)
            {
                slotList[i].gameObject.SetActive(true);
                slotList[i].Setup(availableQuests[i]);
            }
            else
            {
                GameObject newBtnObj = Instantiate(questSlotPrefab, contentParent);
                QuestSlotBtn slotBtn = newBtnObj.GetComponent<QuestSlotBtn>();
                slotBtn.Setup(availableQuests[i]);
                slotList.Add(slotBtn);

            }
        }

        
        SelectQuest(availableQuests[0]);
    }


    public void SelectQuest(QuestSO _quest)
    {
        currentSelectedQuest = _quest;

        questTitleText.text=currentSelectedQuest.questName;
        questDescText.text=currentSelectedQuest.questExplanation;
        questRewardText.text = "보상:" + currentSelectedQuest.RewardMoney + "$";

        Quest activeQuest = QuestManager.Instance.GetActiveQuest(_quest);

        if(activeQuest ==null)
        {
            acceptBtn.SetActive(true);
            completeBtn.SetActive(false);
            questStateText.text = "미수락";
            questStateText.color = Color.white;
        }
        else
        {
            acceptBtn.SetActive(false);
            completeBtn.SetActive(true);
            questStateText.text = "진행중";
            questStateText.color = Color.black;
        }
    }

    public void ThereIsNoQuest()
    {
        questTitleText.text = "퀘스트가 없어요.";
        questDescText.text = "";
        questRewardText.text = "";
        questStateText.text = "";

        acceptBtn.SetActive(false);
        completeBtn.SetActive(false);
    }


    public void AcceptBtn()
    {
        if(currentSelectedQuest != null)
        {
            Quest newQuest = new Quest(currentSelectedQuest);

            QuestManager.Instance.AcceptQuest(newQuest);
            questStateText.text = "진행중";
            questStateText.color = Color.black;

            acceptBtn.SetActive(false);
            completeBtn.SetActive(true) ;
            marketTalk.text = "그럼 잘 부탁하네";

        }
    }

    public void CompleteBtn()
    {  
        if(currentSelectedQuest !=null)
        {
            Quest activeQuest = QuestManager.Instance.GetActiveQuest(currentSelectedQuest);

            if(activeQuest != null)
            {
                if(activeQuest.State == QuestState.CanComplete)
                {
                    QuestUI.Instance.SaveQuestData(null);
                    PlayerInventory.Instance.inventory.RemoveManyItem(activeQuest.questData.goalItem, activeQuest.questData.goalItemCount);
                    Wallet.Instance.GetMoney(activeQuest.questData.RewardMoney);
                    activeQuest.CompleteQuest();
                    QuestManager.Instance.QuestComplete(activeQuest);                 
                    marketTalk.text = "들어줘서 고맙네.";

                    OpenQuestUI(currentOfferList);
                   
                }
                else
                {
                    marketTalk.text = "아직 부족한것 같구만.";
                }
            }
        }
    }

    

   

}
