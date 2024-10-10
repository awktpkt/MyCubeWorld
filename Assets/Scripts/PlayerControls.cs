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

    public GameObject raycastSource;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
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
        //transform.Rotate(0, Input.GetAxis("Mouse X") * rotateSpeed, 0);

        //playerCamera.Rotate(-Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0);
        //if (playerCamera.localRotation.eulerAngles.y != 0)
        //{
        //    playerCamera.Rotate(Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0);
        //}

        if (isFlying && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            speed += 0.005f;
        }
        else if(isFlying && !Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
        {
            speed = 6.0f;
        }

        if(!isFlying && speed != 6.0f)
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

        Vector3 upperRaySource = new Vector3(raycastSource.transform.position.x, raycastSource.transform.position.y + MeshBuilder.BlockScale, raycastSource.transform.position.z);
        float rayDistance = MeshBuilder.BlockScale * 1.5f;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + MeshBuilder.BlockScale, transform.position.z);

        if (!Physics.Raycast(upperRaySource, transform.forward, rayDistance) && Physics.Raycast(raycastSource.transform.position, transform.forward, out RaycastHit hit1, rayDistance) && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if (hit1.collider.CompareTag("Ground"))
            {
                transform.position = newPos;
            }
        }
        if (!Physics.Raycast(upperRaySource, -transform.forward, rayDistance) && Physics.Raycast(raycastSource.transform.position, -transform.forward, out RaycastHit hit2, rayDistance) && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if (hit2.collider.CompareTag("Ground"))
            {
                transform.position = newPos;
            }
        }
        if (!Physics.Raycast(upperRaySource, transform.right, rayDistance) && Physics.Raycast(raycastSource.transform.position, transform.right, out RaycastHit hit3, rayDistance) && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if (hit3.collider.CompareTag("Ground"))
            {
                transform.position = newPos;
            }
        }
        if (!Physics.Raycast(upperRaySource, -transform.right, rayDistance) && Physics.Raycast(raycastSource.transform.position, -transform.right, out RaycastHit hit4, rayDistance) && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            if (hit4.collider.CompareTag("Ground"))
            {
                transform.position = newPos;
            }
        }

        Debug.DrawRay(raycastSource.transform.position, transform.forward,Color.red);
        Debug.DrawRay(new Vector3(raycastSource.transform.position.x, raycastSource.transform.position.y + MeshBuilder.BlockScale, raycastSource.transform.position.z), transform.forward,Color.blue);
        Debug.DrawRay(raycastSource.transform.position, -transform.forward, Color.red);
        Debug.DrawRay(new Vector3(raycastSource.transform.position.x, raycastSource.transform.position.y + MeshBuilder.BlockScale, raycastSource.transform.position.z), -transform.forward, Color.blue);
        Debug.DrawRay(raycastSource.transform.position, transform.right, Color.red);
        Debug.DrawRay(new Vector3(raycastSource.transform.position.x, raycastSource.transform.position.y + MeshBuilder.BlockScale, raycastSource.transform.position.z), transform.right, Color.blue);
        Debug.DrawRay(raycastSource.transform.position, -transform.right, Color.red);
        Debug.DrawRay(new Vector3(raycastSource.transform.position.x, raycastSource.transform.position.y + MeshBuilder.BlockScale, raycastSource.transform.position.z), -transform.right, Color.blue);

    }
}
