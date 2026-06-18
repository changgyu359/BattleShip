using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    { get { return instance; } }

    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private InventoryUI invenUI;


    private void Awake()
    {
        if(instance==null)
            instance = this;
        else
            Destroy(gameObject);

        inventory.InitInventory();
        invenUI.InitInventoryUI();
    }


    public void GetItem(ItemSO _itemData)
    {
        inventory.AddOneItem(_itemData);
        invenUI.Redraw();
    }


}
