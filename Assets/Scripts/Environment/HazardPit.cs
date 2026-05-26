using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HazardPit : MonoBehaviour, IDamager
{
    public int Damage => 1;

    public float KnockbackForce => 0;

    public bool AttackEnabled => true;

    public bool DamageTarget(IDamageable defender)
    {
        if (!AttackEnabled) return false;
        if (!defender.IsValidTarget()) return false;

        defender.TakeDamage(this, Damage);

        return true;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: After entering the area, slowly pull the Character towards the center
        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter && foundCharacter.IsGrounded && !foundCharacter.IsFalling)
        {
            if (foundCharacter is IDamageable damageable)
                DamageTarget(damageable);

            foundCharacter.transform.position = transform.position;
            foundCharacter.FallIntoPit();
        }
    }
}
