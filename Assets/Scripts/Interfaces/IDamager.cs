using UnityEngine;

public interface IDamager
{
    int Damage { get; }
    float KnockbackForce { get; }
    bool AttackEnabled { get; }

    bool DamageTarget(IDamageable defender);
}
