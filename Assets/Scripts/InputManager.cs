using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;

    public Vector2 movementInput;

    public float verticalInput;
    public float horizontalInput;
    AnimatorManager animatorManager;

    PlayerLocomotion playerLocomotion;
    public Vector2 cameraInput;
    public float moveAmount;
    public float cameraInputX;
    public float cameraInputY;
    public bool sprintInput;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movimento.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();

    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        


        

    }
    private void HandleSprintingInput()
    {
        if (sprintInput && moveAmount > 0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }
}
