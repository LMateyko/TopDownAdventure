using UnityEngine;

/// <summary>
/// Prevent the player from dying to pits while the bridge is active
/// </summary>
[RequireComponent (typeof(BoxCollider2D))]
public class DynamicBridge : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter)
        {
            foundCharacter.IsFloating = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter)
        {
            foundCharacter.IsFloating = false;
        }
    }
}
