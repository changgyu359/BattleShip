using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private IInteractable currentInteractable;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IInteractable interactable))
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out IInteractable interactable))
        {
            if(currentInteractable == interactable)
                currentInteractable = null;
        }
    }

    public void OnInteractWithCurrentTarget()
    {
        if(currentInteractable != null)
            currentInteractable.OnInteract();
    }
}
