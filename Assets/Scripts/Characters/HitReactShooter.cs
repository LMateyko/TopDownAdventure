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
        public Projectile projectile;
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
        Projectile projectile = Instantiate(launchConfig.projectile, transform.position, Quaternion.identity);
        Vector2 launchVelocity = launchConfig.launchDirection;

        if (launchConfig.useHitDirection)
        {
            launchVelocity = source.transform.position - target.transform.position;
        }
        else if (launchConfig.useReverseHitDirection)
        {
            launchVelocity = target.transform.position - source.transform.position;
        }

        projectile.SetOwner(target.transform);
        projectile.SetLaunchVelocity(launchVelocity);
    }

}
