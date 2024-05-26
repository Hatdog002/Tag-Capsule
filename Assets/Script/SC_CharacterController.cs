using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class SC_CharacterController : MonoBehaviourPunCallbacks
{
    [Header("Base Values")]
    public float walkingSpeed; //this will be the basic walk speed of the character
    public float runningSpeed; //this will serve as the character's maximum movement speed aka run
    public float jumpForce; //this will be the character's jumping power
    public float gravity; //this will serve as the world gravity
    //public PlayerType Player;
    public TextMeshProUGUI Interact;
    //public Material newMaterial;

    [Header("Camera Reference")]
    public Camera playerCamera; //this will be referenced to the main camera/ the camera that will serve as the player's vision

    [Header("Camera Rotation")]
    public float lookSpeed = 2.0f; //sets the speed sensitivity
    public float lookXLimit = 45.0f; //the angle of the look up and down

    [Header("Controller Properties")]
    public CharacterController characterController; //reference to the character controller component
    Vector3 moveDirection = Vector3.zero; //identifies the direction for movement
    float rotationX = 0; //this is the base rotation of the character

    [Header("Movement Condition")]
    public bool canMove = true; //this identifies if the character is allowed to move

    // Start is called before the first frame update
    void Start()
    {
       if (!photonView.IsMine)
       { 
            characterController = GetComponent<CharacterController>(); //this automatically gets the character controller component
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
        //this will lock and hide the cursor from the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
       if (!photonView.IsMine)
           return;
        //this is for showing the cursor------------------------------------
        if (Input.GetKey(KeyCode.Z))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //end cursor conditions---------------------------------------------

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //press left shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift); //this will return true, if the lShift is pressed

        //conditions for movement
        // if ? then : else
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        //for the jumping condition
        if(Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpForce;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            //pull the object down
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        //Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); //this limits the angle of the x rotation
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    

}
