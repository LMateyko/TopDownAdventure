using Reflex.Attributes;
using UnityEngine;

public class HazardShootAtPlayer : ProjectileHazard
{
    [Header("Fire at Player Settings")]
    [SerializeField] private float m_fireDelay = 2.5f;
    [SerializeField] private float m_timeOffset = 0f;
    [Space]
    [SerializeField] private Transform m_owner;

    [Inject] readonly private PlayerManager PlayerManager;

    protected override void SetDefaultTime()
    {
        m_launchTimer = m_timeOffset;
    }

    private void Update()
    {
        if (!IsEnabled) return;

        if (m_launchTimer >= m_fireDelay)
        {
            ShootProjectile();
            m_launchTimer = 0;
        }

        m_launchTimer += Time.deltaTime;
    }

    protected override void SetProjectileSettings(Projectile projectile)
    {
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.identity;

        projectile.SetOwner(m_owner);

        if(PlayerManager.Player != null)
        {
            Vector2 direction = PlayerManager.Player.transform.position - transform.position;
            projectile.SetLaunchDirection(direction.normalized);
        }
    }

}
