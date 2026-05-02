using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HazardPit : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: After entering the area, slowly pull the Character towards the center
        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter && foundCharacter.IsGrounded && !foundCharacter.IsFalling)
        {
            foundCharacter.transform.position = transform.position;
            foundCharacter.FallIntoPit();
        }
    }
}
