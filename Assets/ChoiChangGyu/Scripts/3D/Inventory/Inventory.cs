using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Slot[] slots;
    [SerializeField]
    private int slotCount;
    public int SlotCount
    { get { return slots.Length; } }

    private void Awake()
    {
        slots = new Slot[slotCount];

        for(int i = 0; i < slots.Length; i++)
            slots[i] = new Slot();
    }

    
    public void AddOneItem(ItemSO _itemData)
    {
        int index=FindFirstItem(_itemData);

        if (index == -1)
        {
            index = FindFirstEmpty();
            if(index == -1)
                Debug.LogError("인벤토리가 가득합니다!");
            else
                slots[index].SetItem(_itemData);
        }
        else 
            slots[index].SetItem(_itemData);
    }

    public void RemoveOneItem(ItemSO _itemData)
    {
        int index = FindLowerItem(_itemData);
        if (index == -1)
            Debug.LogError("해당 아이템이 존재하지 않습니다!");
        else
            slots[index].ItemDown();
    }

    public void RemoveManyItem(ItemSO _itemData, int _count)
    {
        if(HasItem(_itemData, _count))
        {
            while (_count <= 0)
            {
                int index = FindLowerItem(_itemData);
                int tempCount = slots[index].CurItemCount;
                slots[index].ItemDown(Mathf.Min(_count, tempCount));
                _count -= tempCount;
            }
            
        }
        else
        {
            Debug.Log("아이템이 부족합니다!");
        }
    }

    private bool HasItem(ItemSO _itemData, int _count)
    {
        int itemCount = 0;
        for(int i=0; i < slots.Length; i++)
        {
            if (slots[i].CurItemData == _itemData)
                itemCount += slots[i].CurItemCount;
        }

        return itemCount >= _count;
    }



    private int FindFirstItem(ItemSO _itemData)
    {
        for(int i = 0;i<slots.Length;i++)
        {
            if (slots[i].CurItemData == _itemData && !slots[i].IsFull())
            {
                return i;
            }
        }
        return -1;
    }

    private int FindFirstEmpty()
    {
        for(int i=0;i<slots.Length;i++)
        {
            if (slots[i].IsEmpty())
                return i;
        }
        return -1;
    }

 

    private int FindLowerItem(ItemSO _itemData)
    {
        int index = -1;
        for(int i=0;i<slots.Length;i++)
        {
            if (slots[i].CurItemData == _itemData &&
                (index == -1 || slots[index].CurItemCount > slots[i].CurItemCount))
                index = i;
        }
        return index;
    }

    public Slot GetSlot(int _index)
    { return slots[_index]; }

}
