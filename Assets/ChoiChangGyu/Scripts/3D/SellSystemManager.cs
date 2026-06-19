using UnityEngine;

public class SellSystemManager : MonoBehaviour
{
    private static SellSystemManager instance;
    public static SellSystemManager Instance
    { get { return instance; } }

    [SerializeField]
    private SellSystem[] sellSystems;

    [SerializeField]
    private Inventory inven;
    public Inventory Inven
    {  get { return inven; } }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Culculate()
    {
        for(int i=0; i < sellSystems.Length; i++)
        {
            ItemSO data = sellSystems[i].mySellItem;

            int count=inven.HowManyItem(data);

            sellSystems[i].SetHasCount(count);
        }
    }
}
