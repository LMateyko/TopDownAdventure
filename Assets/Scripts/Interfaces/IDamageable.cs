using UnityEngine;

public interface IDamageable
{
    Transform transform { get; }

    bool IsAlive { get; }
    bool IsGrounded { get; }

    bool IsValidTarget();
    void TakeDamage(IDamager source, int damage);
    void Knockback(Vector2 direction, float force);
}
