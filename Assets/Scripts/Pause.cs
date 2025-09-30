using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [SerializeField]private PlayerInput playerInput;

    private void Awake()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log(playerInput.currentActionMap.name);
            if (playerInput.currentActionMap.name == "Player")
            {
                playerInput.SwitchCurrentActionMap("Pause");
            }
            else if (playerInput.currentActionMap.name == "Pause")
            {
                playerInput.SwitchCurrentActionMap("Player");
            }
        }     
    }
}
