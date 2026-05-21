using UnityEngine;

public interface IDamager
{
    Transform transform { get; }

    int Damage { get; }
    float KnockbackForce { get; }
    bool AttackEnabled { get; }

    bool DamageTarget(IDamageable defender);
}
