using Reflex.Attributes;
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

    [Inject] readonly private PoolManager PoolManager;

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
        Projectile projectile = PoolManager.SpawnObject(launchConfig.projectile.Prefab);
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.identity;

        Vector2 launchDirection = launchConfig.launchDirection;

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
            launchDirection = Quaternion.AngleAxis(hitAngle, Vector3.forward) * launchDirection;
        }

        projectile.SetOwner(source.transform);
        projectile.SetLaunchDirection(launchDirection);
        projectile.SetAttackData(launchConfig.projectile.Damage, launchConfig.projectile.Knockback);
    }

}
