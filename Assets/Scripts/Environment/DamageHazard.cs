using UnityEngine;

public class DamageHazard : MonoBehaviour
{
    [SerializeField] private int m_hazardDamage = 1;
    [SerializeField] private float m_knockbackForce = 5f;
    [SerializeField] private bool m_groundedOnly = true;
    [SerializeField] private BoxCollider2D m_attackCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter && foundCharacter.IsGrounded && !foundCharacter.IsFalling)
        {
            if(!m_groundedOnly || (m_groundedOnly && foundCharacter.IsGrounded))
                DamageTarget(foundCharacter);
        }
    }

    private void DamageTarget(BaseCharacterController defender)
    {
        defender.TakeDamage(m_hazardDamage);

        var contactDirection = (defender.transform.position - transform.position).normalized;
        defender.Knockback(contactDirection, force: m_knockbackForce);
    }
}
