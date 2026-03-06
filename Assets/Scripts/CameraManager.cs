using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
public class CameraManager : MonoBehaviour
{
    InputManager inputManager; // Gerencia as entradas do jogador
    public Transform targetTransform; // O alvo a camera vai seguir
    public Transform cameraPivot; // O pivot da camera, usado para rotacionar a camera
    private Transform cameraTransform;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public LayerMask collisionLayers; // As camadas usadas para detectar colisões da camera

    private Vector3 cameraVectorPosition;
    public float cameraCollisionOffset = 0.2f; // A distância que a camera vai se afastar do objeto quando colidir
    public float cameraLookSpeed = 2f; 
    public float cameraPivotSpeed = 2f;
    public float minimumCollisionOffset = 0.2f; // A distância mínima que a camera pode se aproximar do objeto
    public float minimumPivotAngle = -40f; 
    public float maximumPivotAngle = 40f;
    private float defaultPosition; // A posição padrão da camera, usada para calcular colisões
    public float cameraFollowSpeed = 0.2f;
    public float cameraCollisionRadius = 0.2f; // O raio da esfera usada para detectar colisões da camera


    public float lookAngle; // Camera vai olhar para baixo ou para cima
    public float pivotAngle; // Camera vai olhar para os lados

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        targetTransform = FindFirstObjectByType<PlayerManager>().transform; 
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z; 

    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp // Vai atualizar a posição da camera suavemente em direção ao alvo
            (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition; // Atualiza a posição da camera
    }

    private void RotateCamera()
    {
        lookAngle += (inputManager.cameraInputX * cameraLookSpeed); // Rotaciona a camera para cima ou para baixo
        pivotAngle -= (inputManager.cameraInputY * cameraPivotSpeed); // Rotaciona a camera para os lados
        

        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle; // Aplica a rotação no eixo Y
        Quaternion targetRotation = Quaternion.Euler(rotation); // Cria uma rotação a partir do vetor de rotação
        transform.rotation = targetRotation;
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation; // Aplica a rotação no pivot da camera

    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point); 
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition -= minimumCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

}
