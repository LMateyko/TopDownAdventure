using UnityEngine;

public class DamageHazard : MonoBehaviour, IDamager
{
    [SerializeField] public int Damage { get; } = 1;
    [SerializeField] public float KnockbackForce { get; } = 4f;

    public bool AttackEnabled => true;

    [SerializeField] private bool m_groundedOnly = true;
    [SerializeField] private BoxCollider2D m_attackCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundCharacter = collision.GetComponent<IDamageable>();
        if (foundCharacter != null && foundCharacter.IsGrounded)
        {
            if(!m_groundedOnly || (m_groundedOnly && foundCharacter.IsGrounded))
                DamageTarget(foundCharacter);
        }
    }

    public bool DamageTarget(IDamageable defender)
    {
        if (!AttackEnabled) return false;

        if (!defender.IsValidTarget())
            return false;

        var contactDirection = (defender.transform.position - transform.position).normalized;
        defender.Knockback(contactDirection, force: KnockbackForce);
        defender.TakeDamage(Damage);

        return true;
    }
}
