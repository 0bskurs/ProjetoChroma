using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    CameraManager cameraManager;
    PlayerLocomotion playerLocomotion;

    void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = FindFirstObjectByType<CameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
    }   



    

    // Update is called once per frame
    void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
    private void LateUpdate()
    {

        cameraManager.HandleAllCameraMovement();

        playerLocomotion.isJumping = animator.GetBool("isJumping");

    }
}
