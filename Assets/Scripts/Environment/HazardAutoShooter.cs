using System;
using System.Collections.Generic;
using UnityEngine;

public class HazardAutoShooter : MonoBehaviour, IRoomObject
{
    [SerializeField] float m_idleTime = 5f;
    [SerializeField] float m_preparedTime = .25f;
    [SerializeField] float m_timeOffset = 0f;

    [SerializeField] int m_projectileDamage;
    [SerializeField] float m_projectileKnockback;

    [Space]
    [SerializeField] Sprite m_idleSprite;
    [SerializeField] Sprite m_preparedSprite;

    [SerializeField] Projectile m_projectile;

    [Space]
    [SerializeField] SpriteRenderer m_renderer;

    private float m_launchTimer = 0f;
    private List<Projectile> m_activeProjectiles = new List<Projectile>();

    #region iRoomObject Implementation
    public bool IsEnabled { get; private set; }
    public bool PersistantRespawn => true;
    public Action<IRoomObject> OnDestroy { get; set; }

    public void EnableObject()
    {
        IsEnabled = true;
        m_launchTimer = m_timeOffset;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_launchTimer = m_timeOffset;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsEnabled) return;

        if(m_launchTimer < m_idleTime)
        {
            m_renderer.sprite = m_idleSprite;
        }
        else if(m_launchTimer < m_idleTime + m_preparedTime)
        {
            m_renderer.sprite = m_preparedSprite;
        }
        else
        {
            // Launch Projectile
            // TODO: Pull from pool
            Projectile projectile = Instantiate(m_projectile);
            projectile.RotateToTransform(transform);
            projectile.SetAttackData(m_projectileDamage, m_projectileKnockback);

            projectile.transform.position += projectile.DirectionVector * .85f;
            m_activeProjectiles.Add(projectile);
            projectile.OnDestroy += ClearProjectile;

            m_launchTimer = 0;
            m_renderer.sprite = m_idleSprite;
        }

        m_launchTimer += Time.deltaTime;
    }

    private void ClearProjectile(Projectile projectile)
    {
        m_activeProjectiles.Remove(projectile);
    }
}
