using UnityEngine;

public class PlayerLoutine : MonoBehaviour
{
    [SerializeField]
    private Inventory playerInventory;


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("DropItem"))
        {
            DropItem item = other.GetComponent<DropItem>();
            if (item != null&&item.Collectable&&!item.IsFollowing)
            {
                if(playerInventory.CanAddItem(item.ItemData))
                    item.StartFollowing(transform.parent);
            }
        }
    }
}
