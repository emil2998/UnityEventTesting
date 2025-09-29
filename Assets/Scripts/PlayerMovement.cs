using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float speed;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float walkSpeed = 5f;
    private bool canShoot = true;
    private bool isCrouched = false;
    private bool wantToShoot = false;
    private bool wantToCrouch = false;
    private bool wantToPickup = false;
    private bool couroutineRunning = false;
    private bool canPickup = false;
    [SerializeField] private Transform hands;

    private GameObject pickupReference;
    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

    }

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
        if (context.canceled)
        {
            wantToPickup = false;
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

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        Look();
        if (wantToShoot && canShoot && !couroutineRunning)
        {
            canShoot = false;
            StartCoroutine(Shoot());
        }
        Crouch();

        if (canPickup && wantToPickup)
        {
            PickupItem();
        }
        else if(!wantToPickup)
        {
            ReleaseItem();
        }

    }

    private void Crouch()
    {
        if (wantToCrouch)
        {
            isCrouched = true;



        }
        else
        {
            isCrouched = false;

        }
    }

    private IEnumerator Shoot()
    {
        couroutineRunning = true;
        Debug.Log("shjooting");
        yield return new WaitForSeconds(0.1f);
        canShoot = true;
        couroutineRunning = false;

    }

    private void Look()
    {

        transform.Rotate(Vector3.up, lookInput.x);
    }
    private void Move()
    {

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        if (isCrouched)
        {
            speed = crouchSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
        playerRigidbody.linearVelocity = moveDirection * speed;

    }

    private void PickupItem()
    {

        pickupReference.transform.SetParent(hands);
        pickupReference.transform.localPosition = Vector3.zero;
        Rigidbody rigidbody = pickupReference.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

    }

    private void ReleaseItem()
    {

        hands.transform.DetachChildren();
        Rigidbody rigidbody = pickupReference.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        // pickupReference.transform.localPosition = Vector3.zero;

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
            pickupReference = null;
        }
    }
}
