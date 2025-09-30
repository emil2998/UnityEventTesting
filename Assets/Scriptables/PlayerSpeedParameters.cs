using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpeedParameters", menuName = "Scriptables/PlayerSpeedParameters")]
public class PlayerSpeedParameters: ScriptableObject
{
    public float normalMovementSpeed;
    public float crouchedMovementSpeed;
    public float rotationSpeed;
    public float jumpStrength;
    
}
