using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidBody;

    public float walkingSpeed = 1.5f;
    public float sprintingSpeed = 7f;
    public float runningSpeed = 5f;
    public bool isSprinting;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float rotationSpeed;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidBody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;

    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.movementInput.y;
        moveDirection += cameraObject.right * inputManager.movementInput.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }
        moveDirection *= movementSpeed;
        
        Vector3 movementVelocity = moveDirection;
        playerRigidBody.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * inputManager.movementInput.y;
        targetDirection += cameraObject.right * inputManager.movementInput.x;
        targetDirection.Normalize();
        targetDirection.y = 0;

        
        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed*Time.deltaTime);
        transform.rotation = playerRotation;
    }

    
}
