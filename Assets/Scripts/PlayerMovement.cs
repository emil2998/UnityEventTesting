using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float speed;
    [SerializeField] private PlayerSpeedParameters playerSpeedParameters;

    private float lookSensitivity;
    private float jumpStrength;
    private bool canShoot = true;
    private bool wantToShoot = false;
    private bool wantToCrouch = false;

    [SerializeField] private Transform hands;
    private GameObject pickupReference;
    private GameObject objectInHands;
    private Transform holdTarget;
    private bool canPickup = false;
    public bool isHolding = false;
    private bool wantToPickup = false;

    private bool coroutineRunning = false;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    #region InputCallbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnMouseLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            wantToShoot = true;
        }
        if (context.canceled)
        {
            wantToShoot = false;
        }
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            wantToPickup = true;
        }
        else if (context.canceled)
        {
            wantToPickup = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            wantToCrouch = true;
        }
        if (context.canceled)
        {
            wantToCrouch = false;
        }
    }

    #endregion
    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        Look();
        if (wantToShoot && canShoot && !coroutineRunning)
        {
            canShoot = false;
            StartCoroutine(Shoot());
        }

        if (wantToPickup)
        {
            if (canPickup && pickupReference != null && objectInHands == null)
            {
                PickupItem(pickupReference);
            }
        }
        else
        {
            if (objectInHands != null)
            {
                ReleaseHeldItem();
            }
        }
    }
    private void LateUpdate()
    {
        if (isHolding && objectInHands != null && holdTarget != null)
        {
            objectInHands.transform.SetPositionAndRotation(holdTarget.position, holdTarget.rotation);
        }
    }


    #region Movement & Look


    private void Look()
    {
        lookSensitivity = playerSpeedParameters.rotationSpeed;
        float yaw = lookInput.x * lookSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up, yaw);
    }
    private void Move()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        if (wantToCrouch)
        {
            speed = playerSpeedParameters.crouchedMovementSpeed;
        }
        else
        {
            speed = playerSpeedParameters.normalMovementSpeed;
        }
        Vector3 finalVelocity = moveDirection * speed;
        finalVelocity.y = playerRigidbody.linearVelocity.y;
        playerRigidbody.linearVelocity = finalVelocity;
    }

    private void Jump()
    {
        if (playerRigidbody.linearVelocity.y != 0)
        {
            return;
        }
        jumpStrength = playerSpeedParameters.jumpStrength;
        playerRigidbody.AddForce(transform.up * jumpStrength);
    }

    private void Crouch()
    {
        if (wantToCrouch && playerRigidbody.linearVelocity.y == 0)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        }
        else if (!wantToCrouch)
        {
            transform.localScale = Vector3.one;
        }
    }

    #endregion

    #region Shooting
    private IEnumerator Shoot()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.1f);
        canShoot = true;
        coroutineRunning = false;
    }

    #endregion

    #region Pickup / Release
    private void PickupItem(GameObject pickup)
    {
        if (pickup == null) return;

        if (objectInHands != null)
        {
            ReleaseHeldItem();
        }

        Rigidbody pickupRB = pickup.GetComponent<Rigidbody>();
        if (pickupRB != null)
        {
            pickupRB.isKinematic = true;
            pickupRB.useGravity = false;
        }

        objectInHands = pickup;
        holdTarget = hands;
        isHolding = true;
    }

    private void ReleaseHeldItem()
    {
        if (objectInHands == null) return;

        Rigidbody pickupRB = objectInHands.GetComponent<Rigidbody>();


        if (pickupRB != null)
        {
            pickupRB.isKinematic = false;
            pickupRB.useGravity = true;
        }

        objectInHands.transform.position = hands.position + hands.forward * 0.5f;
        objectInHands = null;
        holdTarget = null;
        isHolding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Pickup pickupItem))
        {
            canPickup = true;
            pickupReference = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Pickup pickupItem))
        {
            canPickup = false;

            if (objectInHands == null && pickupReference == other.gameObject)
            {
                pickupReference = null;
            }
        }
    }

    #endregion
}
