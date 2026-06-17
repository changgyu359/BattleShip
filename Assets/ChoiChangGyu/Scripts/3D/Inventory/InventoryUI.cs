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

    private void Start()
    {
        slotUIArray = new SlotUI[inven.SlotCount];
        for(int i = 0;i<slotUIArray.Length;i++)
        {
            SlotUI slotUI = Instantiate(slotPF,inventoryPannel.transform);
            slotUIArray[i] = slotUI;
        }
        Redraw();
    }

    public void Redraw()
    {
        for (int i = 0; i < slotUIArray.Length; i++)
            slotUIArray[i].ShowSlot(inven.GetSlot(i));
    }
}
