
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidBody;
    AnimatorManager animatorManager;

    #region Horizontal Movement
    public float walkingSpeed = 1.5f;
    public float sprintingSpeed = 7f;
    public float runningSpeed = 5f;
    public bool isSprinting;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float rotationSpeed;
    [SerializeField] private float startingSpeed = 4f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float directionChangeBoost = 80f;
    [SerializeField] private float maxSpeed = 12f;
    private Vector3 currentVelocity;
    #endregion
    #region Jump

    [SerializeField] private Transform foot;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpStrength;
    public bool isJumping;
    #endregion
    private void Update()
    {
        Debug.DrawRay(foot.position, Vector3.down * 0.1f, Color.red);
    }

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidBody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector3 inputDirection = cameraObject.forward * inputManager.movementInput.y;
        inputDirection += cameraObject.right * inputManager.movementInput.x;
        inputDirection.y = 0;

        float inputMagnitude = inputDirection.magnitude;
        if (inputMagnitude > 0.01f)
            inputDirection.Normalize();

        Vector3 targetVelocity = inputDirection * maxSpeed;

        if (inputMagnitude > 0.01f)
        {
            if (currentVelocity.sqrMagnitude < 0.01f)
            {
                // Set starting speed in input direction
                currentVelocity = inputDirection * startingSpeed;
            }
            else
            {
                float alignment = Vector3.Dot(currentVelocity.normalized, targetVelocity.normalized);

                if (alignment < 0f)
                {
                    currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 0.5f);
                }
                else if (alignment < 0.5f)
                {
                    Vector3 velocityAlongOldDir = Vector3.Project(currentVelocity, targetVelocity.normalized);
                    Vector3 lateralVelocity = currentVelocity - velocityAlongOldDir;
                    lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, directionChangeBoost * Time.deltaTime);
                    currentVelocity = velocityAlongOldDir + lateralVelocity;
                }
            }

            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.deltaTime
            );
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                deceleration * Time.deltaTime
            );
        }

        Vector3 velocity = playerRigidBody.linearVelocity;
        velocity.x = currentVelocity.x;
        velocity.z = currentVelocity.z;
        playerRigidBody.linearVelocity = velocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = cameraObject.forward * inputManager.movementInput.y;
        targetDirection += cameraObject.right * inputManager.movementInput.x;
        targetDirection.y = 0;

        if (targetDirection.sqrMagnitude < 0.001f)
            targetDirection = transform.forward;

        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        float dynamicSpeed = Mathf.Clamp(rotationSpeed * angleDifference, 0.1f, rotationSpeed * 10f);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            dynamicSpeed * Time.deltaTime
        );
    }


    public void HandleJumping()
    {
        if (CheckGrounding() == true)
        {

            animatorManager.animator.SetBool("isJumping", true);
            
            float jumpVelocity = Mathf.Sqrt(jumpStrength * -2f * Physics.gravity.y);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpVelocity;
            playerRigidBody.linearVelocity = playerVelocity;
        }
    }

    private bool CheckGrounding()
    {
        
        if (Physics.Raycast(foot.position, Vector3.down, 0.1f, groundLayer))
        {
            
            return true;
        }
        else
        {
            return false;
        }

        

       
    }



    
}
