using TMPro;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private TMP_Text pickupText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement playerMovement))
        {
            pickupText.text = "HOLD E TO CARRY ITEM";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement playerMovement))
        {
            if (playerMovement.isHolding)
            {
                pickupText.text = "";
            }
            else
            {
                pickupText.text = "HOLD E TO CARRY ITEM";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement playerMovement))
        {
            pickupText.text = "";
        }
    }
}
