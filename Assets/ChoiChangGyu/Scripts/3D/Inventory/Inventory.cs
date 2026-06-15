using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Slot[] slots;
    private int slotCount;

    private void Awake()
    {
        slots = new Slot[slotCount];

        for(int i = 0; i < slots.Length; i++)
            slots[i] = new Slot();
    }

    


}
