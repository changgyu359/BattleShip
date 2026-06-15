using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
        { get { return instance; } }

    [SerializeField] private BoardManager player1Board;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField]
    private GameObject multiParent;

    
    public GameObject ReadyBtn;
    public GameObject shipsPan;

    private bool isPressRestartBtn=false;
    [Header("게임플레이 관련")]
    public GameObject gamePlayParent;
    [SerializeField]
    private TextMeshProUGUI whosTurn;

   

    [Header("게임오버 패널")]
    public GameObject gameoverPannel;
    [SerializeField]
    private TextMeshProUGUI whoIsWin;
    [SerializeField]
    private TextMeshProUGUI restartVote;

    public GameObject MultiParent 
    { get { return multiParent; } }

    public void ServerButton()
    {
        
        _=TCPManager.Instance.StartServerAsync();
    }

    public void ClientButton()
    {
        
        _=TCPManager.Instance.ConnectClientAsync();
    }

    public void ReadyButton()
    {
        if (player1Board.PlaceCount < player1Board.RemainShips)
        {
            Debug.Log("배를 모두 배치해주세요!");
            return;
        }

        ReadyBtn.gameObject.SetActive(false);
        int myId = TCPManager.Instance.IsServer ? 1 : 2;
        TCPManager.Instance.SendReadyDataEvnet(myId);
        BattleshipManger.Instance.PressReadyButton(myId);
        BattleshipManger.Instance.Player1Board.isReady = true;
    }

    public void RestartButton()
    {
        if (!isPressRestartBtn)
        {
            isPressRestartBtn = true;
            TCPManager.Instance.SendVoteDataEvent();
            BattleshipManger.Instance.VoteRetry();

        }
    }

    public void ResetRestartBtn()
    {
        isPressRestartBtn = false;
    }

    public void ExitBtn()
    {
        if(TCPManager.Instance!=null)
            TCPManager.Instance.Disconnect();
        ReturnToLobby();
    }


    public void SetWinner(string _str)
    {
        whoIsWin.text = _str;
    }

    public void SetVote(int _votes)
    {
        restartVote.text = "Restart Vote:" + _votes;
    }

    public void SetWhosTurnText(bool isMyTurn)
    {
        whosTurn.text = isMyTurn ? "My turn" : "Opponent turn";
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
}
