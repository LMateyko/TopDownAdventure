using System;
using UnityEngine;

public class HitReactShooter : MonoBehaviour
{
    [Serializable]
    public struct ProjectileReactionConfig
    {
        public bool useHitDirection;
        public bool useReverseHitDirection;
        public Vector2 launchDirection;
        public ProjectileData projectile;
    }

    [SerializeField] private ProjectileReactionConfig[] m_onHitConfigs;
    [SerializeField] private ProjectileReactionConfig[] m_onDeathConfigs;

    public void LaunchHitProjectiles(IDamager source, IDamageable target)
    {
        foreach(var config in m_onHitConfigs)
        {
            LaunchForConfig(config, source, target);
        }
    }

    public void LaunchDeathProjectiles(IDamager source, IDamageable target)
    {
        foreach (var config in m_onDeathConfigs)
        {
            LaunchForConfig(config, source, target);
        }
    }

    private void LaunchForConfig(ProjectileReactionConfig launchConfig, IDamager source, IDamageable target)
    {
        // TODO: Use Pool
        Projectile projectile = Instantiate(launchConfig.projectile.Prefab, transform.position, Quaternion.identity);
        Vector2 launchVelocity = launchConfig.launchDirection;

        if (launchConfig.useHitDirection || launchConfig.useReverseHitDirection)
        {
            var hitDirectionVector = Vector2.zero;

            if (launchConfig.useHitDirection)
            {
                hitDirectionVector = source.transform.position - target.transform.position;
            }
            else if (launchConfig.useReverseHitDirection)
            {
                hitDirectionVector = target.transform.position - source.transform.position;
            }

            var hitAngle = Vector2.SignedAngle(Vector2.right, hitDirectionVector);
            launchVelocity = Quaternion.AngleAxis(hitAngle, Vector3.forward) * launchVelocity;
        }

        projectile.SetOwner(target.transform);
        projectile.SetLaunchVelocity(launchVelocity);
        projectile.SetAttackData(launchConfig.projectile.Damage, launchConfig.projectile.Knockback);
    }

}
