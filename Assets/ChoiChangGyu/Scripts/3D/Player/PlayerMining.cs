using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public void TryMine()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1.0f, 1.5f);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Ore"))
            {
                hitCollider.GetComponent<Ore>().TakeDamage();
                break;
            }

        }
    }
}
