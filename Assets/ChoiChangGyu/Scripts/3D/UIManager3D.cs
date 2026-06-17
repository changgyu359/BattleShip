using UnityEngine;

public class UIManager3D : MonoBehaviour
{
    private static UIManager3D instance;
    public static UIManager3D Instance
    { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
           instance = this; 
        else
            Destroy(gameObject);
    }


    [SerializeField]
    private GameObject invenPannel;

    public void ToggleInventory()
    {
        bool isActive = invenPannel.activeSelf;
        invenPannel.SetActive(!isActive);
    }

}
