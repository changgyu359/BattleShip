using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private static PlayerControl instance;
    public static PlayerControl Instance
    { get { return instance; } }
   
    private Rigidbody rb;

    private float m_speed = 5f;
    private Vector3 moveDirection;
    public Vector3 MoveDirection
        { get { return moveDirection; } }
    public bool IsInteracting = false;

    private PlayerInteract interactor;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        interactor = GetComponent<PlayerInteract>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            UIManager3D.Instance.ToggleInventory();

        if(IsInteracting)
        {
            moveDirection = Vector3.zero;
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(interactor!=null)
            {
                interactor.OnInteractWithCurrentTarget();
                IsInteracting = true;
            }
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3 (moveX, 0, moveZ).normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveDirection.x * m_speed, rb.linearVelocity.y, moveDirection.z * m_speed);
    
        if(moveDirection!=Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection,Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 0.15f);
        }
    
    }
}
