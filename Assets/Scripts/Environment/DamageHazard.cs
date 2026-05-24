using UnityEngine;

public class DamageHazard : MonoBehaviour, IDamager
{
    [SerializeField] public int m_damage = 1;
    [SerializeField] public float m_knockbackForce = 4f;

    public int Damage  => m_damage;
    public float KnockbackForce => m_knockbackForce;

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
        defender.TakeDamage(this, Damage);

        return true;
    }
}
