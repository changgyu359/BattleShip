using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemSO itemData;

    private string itemName;
    private int max;
    private int sellPrice;

    private void Start()
    {
        itemName=itemData.itemName;
        max=itemData.max;
        sellPrice=itemData.sellPrice;
    }
}
