using UnityEngine;

public class UIManager3D : MonoBehaviour
{
    private static UIManager3D instance;
    public static UIManager3D Instance
    { get { return instance; } }

    [SerializeField]
    private GameObject invenParrent;

    private void Awake()
    {
        if (instance == null)
           instance = this; 
        else
            Destroy(gameObject);
    }

   

    

    public void ToggleInventory()
    {
        bool isActive = invenParrent.activeSelf;
        PlayerInventory.Instance.invenUI.Redraw();
        PlayerInventory.Instance.invenUI.SetMoney();
        invenParrent.SetActive(!isActive);

    }

}
