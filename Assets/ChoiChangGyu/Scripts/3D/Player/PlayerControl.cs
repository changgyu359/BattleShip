using UnityEngine;

public class PlayerControl : MonoBehaviour
{
   
    private Rigidbody rb;

    private float m_speed = 5f;
    private Vector3 moveDirection;
    public Vector3 MoveDirection
        { get { return moveDirection; } }
    public bool IsInteracting = false;

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsInteracting)
        {
            moveDirection = Vector3.zero;
            return;
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
