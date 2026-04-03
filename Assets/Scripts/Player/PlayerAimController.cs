using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerAimController : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rotationSpeed = 720f;

    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();

        if (visualRoot == null)
        {
            visualRoot = transform;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        AimToMouse();
    }

    private void AimToMouse()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(inputHandler.LookScreenPosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDirection = hitPoint - transform.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            visualRoot.rotation = Quaternion.RotateTowards(
                visualRoot.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}