using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    { get { return instance; } }

    
    public Inventory inventory;

    public InventoryUI invenUI;

    


    private void Awake()
    {
        if(instance==null)
            instance = this;
        else
            Destroy(gameObject);

        
    }

    private void Start()
    {
        inventory.InitInventory();
        invenUI.InitInventoryUI();
    }


    public bool GetItem(ItemSO _itemData)
    {
        bool result=inventory.AddOneItem(_itemData);
        invenUI.Redraw();
        return result;
    }


}
