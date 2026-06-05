using UnityEngine;

public class BattleshipManger : MonoBehaviour
{
    private static BattleshipManger instance;
    public static BattleshipManger Instance
    {  get { return instance; } }
    
    private int life = 5;


    public void OnShipSunk()
    {
        life--;
        if(life <= 0)
            GameOver();
    }

    public void GameOver()
    {
        BoardManager.Instance.isPlaying = false;
        Debug.Log("게임종료");
    }



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


}
