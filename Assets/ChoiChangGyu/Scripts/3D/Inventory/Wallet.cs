using UnityEngine;

public class Wallet : MonoBehaviour
{
    private static Wallet instance;
    public static Wallet Instance
    { get { return instance; } }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private int money=0;
    
    public int Money
    {  get { return money; } }

    public void GetMoney(int _money)
    {
        money += _money;
    }
}
