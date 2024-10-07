using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float rotateSpeed = 0.8f;
    public float gravity = 20.0f;

    private Vector3 moveDirection = Vector3.zero;

    private bool isFlying;

    private CharacterController controller;
    private Transform playerCamera;

    public GameObject raycastSource;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>().transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && isFlying)
        {
            isFlying = !isFlying;
            gravity = 20.0f;
        }
        else if(Input.GetKeyDown(KeyCode.F) && !isFlying)
        {
            isFlying = !isFlying;
            gravity = 0.0f;
        }
        transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed, 0);

        playerCamera.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0);
        if (playerCamera.localRotation.eulerAngles.y != 0)
        {
            playerCamera.Rotate(Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0);
        }

        if (isFlying && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            speed += 0.005f;
        }
        else if(isFlying && !Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
        {
            speed = 6.0f;
        }

        moveDirection = new Vector3(Input.GetAxis("Horizontal") * speed, moveDirection.y, Input.GetAxis("Vertical") * speed);
        moveDirection = transform.TransformDirection(moveDirection);

        if (controller.isGrounded && !isFlying)
        {
            if (Input.GetButton("Jump")) moveDirection.y = jumpSpeed;
            else moveDirection.y = 0;
        }
        
        if(isFlying)
        {
            if (Input.GetButton("Jump")) moveDirection.y = jumpSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) moveDirection.y = -jumpSpeed;
            if(!Input.GetButton("Jump") && !Input.GetKey(KeyCode.LeftShift)) moveDirection.y = 0;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        if (!Physics.Raycast(new Vector3(raycastSource.transform.position.x,raycastSource.transform.position.y+ChunkRenderer.BlockScale,raycastSource.transform.position.z), transform.forward, out RaycastHit hit1, ChunkRenderer.BlockScale * 1.5f) && Physics.Raycast(raycastSource.transform.position, transform.forward, out RaycastHit hit2, ChunkRenderer.BlockScale*1.5f) && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if (hit2.collider.CompareTag("Ground"))
            {
                Vector3 newPos = new Vector3(transform.position.x,transform.position.y + ChunkRenderer.BlockScale,transform.position.z);
                transform.position = newPos;
            }
        }

        Debug.DrawRay(raycastSource.transform.position, transform.forward,Color.red);
        Debug.DrawRay(new Vector3(raycastSource.transform.position.x, raycastSource.transform.position.y + ChunkRenderer.BlockScale, raycastSource.transform.position.z), transform.forward,Color.blue);

    }
}
