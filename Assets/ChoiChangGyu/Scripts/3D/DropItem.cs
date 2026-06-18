using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField]
    private GameObject[] models;

    [SerializeField]
    private ItemSO itemData;

    private int myIndex = -1;

    private bool isFollowing = false;
    private Transform targetPlayer;

    private float currentSpeed = 10f;

    private bool collectable=false;
    public bool Collectable
    { get { return collectable; } }


    private Rigidbody rb;
    private Collider col;


    public void SetUp(OreSO _oreData)
    {
        if(myIndex!=-1)
            models[myIndex].gameObject.SetActive(false);

        models[_oreData.oreIndex].gameObject.SetActive(true);
        myIndex=_oreData.oreIndex;

        itemData = _oreData.itemData;
    }

    public void StartFollowing(Transform _player)
    {
        isFollowing = true;
        targetPlayer = _player;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if(col != null)
            col.enabled = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if(!isFollowing||targetPlayer ==null) return;


        transform.position = Vector3.MoveTowards
            (transform.position,targetPlayer.position, currentSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position,targetPlayer.position)<0.5f)
        {
            PlayerInventory.Instance.GetItem(itemData);
            ResetItem();
            DropItemManager.Instance.ReturnToPool(this);
        }
    }

    private void OnEnable()
    {
        collectable = false;
        Invoke("CanCollect", 2f);
    }

    private void CanCollect()
    {
        collectable = true;
    }

    private void ResetItem()
    {
        isFollowing = false;

        
        if(rb!= null) rb.isKinematic = false;

        
        if (col != null) col.enabled = true;
    }

    public void PopUp()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float jumpForce = 5f;
        float spread = 2f;
        Vector3 force = new Vector3(Random.Range(-spread, spread), jumpForce, Random.Range(-spread, spread));

        rb.AddForce(force,ForceMode.Impulse);
        Debug.Log("이게 발동한거임");
    }
}
