using UnityEngine;

public class PlayerLoutine : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("DropItem"))
        {
            DropItem item = other.GetComponent<DropItem>();
            if (item != null&&item.Collectable)
                item.StartFollowing(transform.parent);
        }
    }
}
