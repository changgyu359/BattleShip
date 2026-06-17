using UnityEngine;

public class Slot
{
    private ItemSO curItemData;
    public ItemSO CurItemData
    { get { return curItemData; } }

    private int curItemCount=0;
    public int CurItemCount
    { get { return curItemCount; } }


    public void SetItem(ItemSO _data)
    {
        curItemData = _data;
        curItemCount++;
    }

    public void SetItem(ItemSO _data,int _count)
    {
        curItemData = _data;
        curItemCount+=_count;
    }

    public void ItemUp(int _count)
    { 
        if (IsFull())
        {
            Debug.LogError("ø©±‚ ≤À√°Ωø");
            return;
        }
        curItemCount++;
    }

    
    public void ItemDown()
    {
        if(IsEmpty())
        {
            Debug.Log("æ∆¿Ã≈€ ¿æ¥Ÿ");
            return;
        }
        curItemCount--;
        if(IsEmpty()) 
            curItemData=null;
    }

    public void ItemDown(int _count)
    {
        if (IsEmpty())
        {
            Debug.LogError("æ∆¿Ã≈€ ¿æ¥Ÿ");
            return;
        }
        else if(curItemCount <_count)
        {
            Debug.LogError("æ∆¿Ã≈€ ±◊∏∏≈≠ ¿æ¥Ÿ");
        }

        curItemCount -= _count;
        if (IsEmpty())
            curItemData = null;
    }

    public void Clear()
    {
        curItemData = null;
        curItemCount = 0;
    }

    public bool IsFull()
    { return curItemCount == curItemData.max; }

    public bool IsEmpty()
    { return curItemCount <= 0; }

    public int RemainToFull()
    { return curItemData.max - curItemCount; }
}
