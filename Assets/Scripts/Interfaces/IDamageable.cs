using UnityEngine;

public interface IDamageable
{
    Transform transform { get; }

    bool IsAlive { get; }
    bool IsGrounded { get; }

    bool TakeDamage(int damage);
    void Knockback(Vector2 direction, float force);
}
