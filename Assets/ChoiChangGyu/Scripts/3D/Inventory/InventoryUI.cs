using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private SlotUI slotPF;
    [SerializeField]
    private GameObject inventoryPannel;
    [SerializeField]
    private Inventory inven;

    private SlotUI[] slotUIArray;

    [SerializeField]
    private TextMeshProUGUI currentMoney;
 

    public void InitInventoryUI()
    {
        slotUIArray = new SlotUI[inven.SlotCount];
        for (int i = 0; i < slotUIArray.Length; i++)
        {
            SlotUI slotUI = Instantiate(slotPF, inventoryPannel.transform);
            slotUIArray[i] = slotUI;
        }
        Redraw();
        SetMoney();
    }

    public void Redraw()
    {
        for (int i = 0; i < slotUIArray.Length; i++)
            slotUIArray[i].ShowSlot(inven.GetSlot(i));
    }

    public void SetMoney()
    {
        currentMoney.text="¼ÒÁö±Ý:"+ Wallet.Instance.Money+"$";
    }
}
