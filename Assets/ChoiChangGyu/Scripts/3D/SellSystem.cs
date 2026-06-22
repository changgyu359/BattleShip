using TMPro;
using UnityEngine;

public class SellSystem : MonoBehaviour
{
    [Header("텍스트")]
    [SerializeField]
    private TextMeshProUGUI hasCountText;
    [SerializeField]
    private TextMeshProUGUI sellCountText;
    [SerializeField]
    private TextMeshProUGUI totalSellPriceText;

    private int hasCount;
    private int sellCount=1;
    [SerializeField]
    private int sellPrice;

    public ItemSO mySellItem;

    public void SetHasCount(int _count)
    {
        hasCount = _count;
        hasCountText.text = "보유량:" + hasCount;
    }

    public void MinusSellCount()
    {
        if (sellCount > 1)
        {
            sellCount--;
            sellCountText.text = sellCount.ToString();
            totalSellPriceText.text = (sellPrice*sellCount).ToString()+"$";
        }
    }

    public void PlusSellCount()
    {
        if(sellCount<hasCount)
        {
            sellCount++;
            sellCountText.text = sellCount.ToString();
            totalSellPriceText.text = (sellPrice * sellCount).ToString() + "$";
        }
    }

    public void Sell()
    {
        SellSystemManager.Instance.Inven.RemoveManyItem(mySellItem, sellCount);
        int count = SellSystemManager.Instance.Inven.HowManyItem(mySellItem);
        SetHasCount(count);
        Wallet.Instance.GetMoney(sellCount * sellPrice);
        PlayerInventory.Instance.invenUI.Redraw();
        PlayerInventory.Instance.invenUI.SetMoney();
        
        ResetSystem();
    }

    private void ResetSystem()
    {
        sellCount = 1;
        sellCountText.text=sellCount.ToString();
        totalSellPriceText.text=sellPrice.ToString()+"$";
    }
}
