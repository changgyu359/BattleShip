using UnityEngine;


public enum GameState
{
    placement,
    player1Turn,
    player2Turn,
    GameOver
}
public class BattleshipManger : MonoBehaviour
{
    private static BattleshipManger instance;
    public static BattleshipManger Instance
    {  get { return instance; } }


    private GameState currentState;
    public GameState CurrentState
    { get { return currentState; } }


    [Header("플레이어의 보드들")]
    [SerializeField]
    private BoardManager player1Board;
    [SerializeField]
    private BoardManager player2Board;

    public BoardManager Player1Board
    { get { return player1Board; } }

    [SerializeField]
    private GameObject shipParent;
    public GameObject ShipParent
    { get { return shipParent; } }  

    private bool isPlyaer1Ready = false;
    private bool isPlyaer2Ready = false;
    private int retryCount = 0;






    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        currentState = GameState.placement;
    }


    public void CheckGameOver(BoardManager sunkBoard)
    {
        if(sunkBoard.RemainShips <=0)
        {
            if (sunkBoard == player1Board)
                GameOver(winnerID: 2);
            else if(sunkBoard == player2Board)
                GameOver(winnerID: 1);

        }
    }

    public void GameOver(int winnerID)
    {
        currentState=GameState.GameOver;
        player1Board.isPlaying = false;
        player2Board.isPlaying = false;
        UIManager.Instance.gameoverPannel.SetActive(true);
        if (winnerID == 1)
            UIManager.Instance.SetWinner("You Win!");
        else
            UIManager.Instance.SetWinner("You Lose...");
    }

    public void OnAttackExecuted(bool _isHit)
    {
        if (currentState == GameState.GameOver) return;

        if(currentState==GameState.player1Turn)
        {
            if(!_isHit)
            {
                ChangeState(GameState.player2Turn);
            }
        }
        else if(currentState==GameState.player2Turn)
        {
            if(!_isHit)
            {
                ChangeState(GameState.player1Turn);
            }
        }
    }

 

    private void ChangeState(GameState _state)
    {  
        currentState = _state;
        bool isMyTurn = (TCPManager.Instance.IsServer && _state == GameState.player1Turn) ||
                    (!TCPManager.Instance.IsServer && _state == GameState.player2Turn);

        
        if (_state == GameState.player1Turn || _state == GameState.player2Turn)
        {
            UIManager.Instance.SetWhosTurnText(isMyTurn);
        }
    }

    public void PressReadyButton(int playerID)
    {
        if (currentState != GameState.placement) return;

        if (playerID == 1) isPlyaer1Ready = true;
        if (playerID == 2) isPlyaer2Ready = true;

        if(isPlyaer1Ready&&isPlyaer2Ready)
        {
            if(TCPManager.Instance.IsServer)
            {
                int firstTurn = Random.Range(1, 3);
                TCPManager.Instance.SendTurnSyncData(firstTurn);
                StartGame(firstTurn);
            }
            
        }
    }

    public void StartGame(int firstTurnId)
    {
        if (currentState != GameState.placement) return;

        player1Board.GameStart(isEnemyBoard: false);
        player2Board.GameStart(isEnemyBoard: true);


        player2Board.SetRemainShips(player1Board.RemainShips);
        player2Board.Tilemap.transform.parent.gameObject.SetActive(true);

        
        UIManager.Instance.shipsPan.gameObject.SetActive(false);
        UIManager.Instance.gamePlayParent.gameObject.SetActive(true);


        if (firstTurnId == 1)
            ChangeState(GameState.player1Turn);
        else
            ChangeState(GameState.player2Turn);

    }

    public void VoteRetry()
    {
        retryCount++;
        UIManager.Instance.SetVote(retryCount);

        if (retryCount >= 2)
        {
            TCPManager.Instance.SendForceRestartEvent();
            Restart();
        }
    }

    public void Restart()
    {
        retryCount = 0;
        isPlyaer1Ready=false;
        isPlyaer2Ready=false;

        UIManager.Instance.ResetRestartBtn();
        UIManager.Instance.SetVote(0);
        UIManager.Instance.gameoverPannel.gameObject.SetActive(false);
        player2Board.Tilemap.transform.parent.gameObject.SetActive(false);
        UIManager.Instance.ReadyBtn.gameObject.SetActive(true);
        UIManager.Instance.gamePlayParent.gameObject.SetActive(false);
        player1Board.isReady = false;

        player1Board.ResetBoardState();
        player2Board.ResetBoardState();
        

        player1Board.ResetAllShipPosition();
        UIManager.Instance.shipsPan.gameObject.SetActive(true);

        currentState = GameState.placement;
    }

    

}
