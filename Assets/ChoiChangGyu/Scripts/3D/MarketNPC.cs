using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarketNPC : MonoBehaviour, IInteractable
{

    [SerializeField]
    private GameObject NPCTalkingPannel;
    [SerializeField]
    private GameObject marketPannel;
    [SerializeField]
    private GameObject marketExitBtn;
    [SerializeField]
    private GameObject buttonParent;
    [SerializeField]
    private GameObject questParent;


    [SerializeField]
    private TextMeshProUGUI marketTalk;

    [SerializeField] private List<QuestSO> npcQuests;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    //IInteractable
    public void OnInteract()
    {
        NPCTalkingPannel.SetActive(true);
        buttonParent.SetActive(true);
        anim.SetBool("Talking", true);
        marketTalk.text = "안녕하신가, 힘세고 강한 아침!";
    }

    public void MarketUse()
    {
        marketPannel.SetActive(true);
        SellSystemManager.Instance.Culculate();
        marketTalk.text = "광물을 많이 모아왔는가?";
        buttonParent.SetActive(false);
        marketExitBtn.SetActive(true);
    }

    public void Quest()
    {
        marketTalk.text = "내가 좀 부탁할게 있다네.";
        questParent.SetActive(true);
        NPCQuestUI.Instance.OpenQuestUI(npcQuests);
    }

    public void Talk()
    {
        int rand=Random.Range(0, 3);

        switch(rand)
        {
            case 0:
                marketTalk.text = "하얀색 크리스탈을 많이 가져오게나. 그게 제일 비싸다네.";
                break;
            case 1:
                marketTalk.text = "보라색 자수정은 흔하지만 값은 가장 싸다네.";
                break;
            case 2:
                marketTalk.text = "모든 광물은 한 칸에 2개씩만 가지고 있을 수 있네.";
                break;
        }
    }


    public void ExitInteract()
    {
        NPCTalkingPannel.SetActive(false);
        buttonParent.SetActive(false);
        anim.SetBool("Talking",false);
        PlayerControl.Instance.IsInteracting = false;
    }

    public void MarketExitBtn()
    {
        marketExitBtn.SetActive(false);
        marketPannel.SetActive(false);
        buttonParent.SetActive(true);
        marketTalk.text = "더 필요한게 있는가?";
    }


}
