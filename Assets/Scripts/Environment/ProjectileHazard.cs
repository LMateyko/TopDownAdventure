using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileHazard : MonoBehaviour, IRoomObject
{
    [SerializeField] protected int m_projectileDamage;
    [SerializeField] protected float m_projectileKnockback;

    [Space]
    [SerializeField] protected Sprite m_idleSprite;
    [SerializeField] protected Sprite m_preparedSprite;

    [SerializeField] protected Projectile m_projectile;

    [Space]
    [SerializeField] protected SpriteRenderer m_renderer;

    protected float m_launchTimer = 0f;
    protected List<Projectile> m_activeProjectiles = new List<Projectile>();

    #region iRoomObject Implementation
    public bool IsEnabled { get; private set; }
    public bool PersistantRespawn => true;
    public Action<IRoomObject> OnDestroy { get; set; }

    public void EnableObject()
    {
        IsEnabled = true;
        SetDefaultTime();
        m_renderer.gameObject.SetActive(true);
    }

    public void DisableObject()
    {
        IsEnabled = false;
        m_renderer.gameObject.SetActive(false);

        foreach (var projectile in m_activeProjectiles)
        {
            projectile.OnDestroy -= ClearProjectile;
            Destroy(projectile.gameObject);
        }

        m_activeProjectiles.Clear();
    }
    #endregion

    protected abstract void SetDefaultTime();

    protected void ShootProjectile()
    {
        // Launch Projectile
        // TODO: Pull from pool
        Projectile projectile = Instantiate(m_projectile);
        projectile.RotateToTransform(transform);
        projectile.SetAttackData(m_projectileDamage, m_projectileKnockback);

        projectile.transform.position += projectile.DirectionVector * .85f;
        m_activeProjectiles.Add(projectile);
        projectile.OnDestroy += ClearProjectile;
    }

    private void Start()
    {
        SetDefaultTime();
    }

    private void ClearProjectile(Projectile projectile)
    {
        m_activeProjectiles.Remove(projectile);
    }
}
