using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HazardPit : MonoBehaviour, IDamager
{
    public int Damage => 1;

    public float KnockbackForce => 0;

    public bool AttackEnabled => true;

    private List<BaseCharacterController> m_trackedCharacters = new List<BaseCharacterController>();

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
        if (foundCharacter )
        {
            

            TriggerPit(foundCharacter);
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter && m_trackedCharacters.Contains(foundCharacter))
        {
            m_trackedCharacters.Remove(foundCharacter);
        }
    }

    protected virtual void Update()
    {
        // Track players that were floating to pit them once walking off of the bridge
        if (m_trackedCharacters.Count == 0) return;

        int i = 0;
        while (i < m_trackedCharacters.Count)
        {
            if (!m_trackedCharacters[i].IsFloating)
            {
                TriggerPit(m_trackedCharacters[i], reposition: false);
                m_trackedCharacters.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    private void TriggerPit(BaseCharacterController foundCharacter, bool reposition = true)
    {
        if (!foundCharacter.IsGrounded || foundCharacter.IsFalling)
            return;

        if (foundCharacter.IsFloating)
        {
            m_trackedCharacters.Add(foundCharacter);
            return;
        }

        if (foundCharacter is IDamageable damageable)
            DamageTarget(damageable);

        if(reposition)
            foundCharacter.transform.position = transform.position;

        foundCharacter.FallIntoPit();
    }
}
