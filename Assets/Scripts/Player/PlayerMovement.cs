using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    private const float GRAVITY = -9.81f;


    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private float jumpHeight = 2;
    [SerializeField]
    private float mouseSensitivity = 3;
    [SerializeField]
    private float gravityAffection = 1;
    [SerializeField]
    private float groundCheckRadius = 0.3f;
    [SerializeField]
    private LayerMask groundMask;


    [SerializeField]
    private GameObject groundCheck;
    [SerializeField]
    private Camera cam;


    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private CharacterController cc;
    private bool isGrounded = false;
    private bool inAir = true;

    // Start is called before the first frame update
    void Start()
    {
        if(cam == null)
        {
            throw new MissingReferenceException();
        }
        cc = GetComponent<CharacterController>();


        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        rotation = Vector3.zero;

        rotating();
        moving();

        applyRotation();

    }






    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
    }


    private void rotating()
    {
        float yRot = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float xRot = Input.GetAxisRaw("Mouse Y") * -mouseSensitivity;

        rotation += new Vector3(xRot, yRot, 0f);

    }

    private void moving()
    {
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * y;

        cc.Move(move * speed * Time.deltaTime);

        //Disable the momentum
        if (x == 0 && y == 0)
        {

        }


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY * gravityAffection);
        }

        velocity.y += GRAVITY * gravityAffection * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }


    private void applyRotation()
    {
        transform.Rotate(Vector3.up * rotation.y);
        cam.transform.Rotate(new Vector3(rotation.x, 0f, 0f));
    }

}


