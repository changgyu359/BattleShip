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
            UIManager.Instance.SetWinner("Server");
        else
            UIManager.Instance.SetWinner("Client");
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
    {  currentState = _state; }

    public void PressReadyButton(int playerID)
    {
        if (currentState != GameState.placement) return;

        if (playerID == 1) isPlyaer1Ready = true;
        if (playerID == 2) isPlyaer2Ready = true;

        if(isPlyaer1Ready&&isPlyaer2Ready)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (currentState != GameState.placement) return;

        player1Board.GameStart(isEnemyBoard: false);
        player2Board.GameStart(isEnemyBoard: true);


        player2Board.SetRemainShips(player1Board.RemainShips);
        player2Board.Tilemap.transform.parent.gameObject.SetActive(true);

        UIManager.Instance.ReadyBtn.gameObject.SetActive(false);
        UIManager.Instance.shipsPan.gameObject.SetActive(false);

        Debug.Log("선공:플레이어1");

        ChangeState(GameState.player1Turn);

    }

    public void VoteRetry()
    {
        retryCount++;
        UIManager.Instance.SetVote(retryCount);

        if (retryCount >= 2)
            Restart();
    }

    private void Restart()
    {
        retryCount = 0;
        isPlyaer1Ready=false;
        isPlyaer2Ready=false;

        currentState = GameState.placement;
    }

    public void GoToLobby()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

}
