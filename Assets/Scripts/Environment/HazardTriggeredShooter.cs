using Reflex.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HazardTriggeredShooter : MonoBehaviour, IRoomObject
{
    [SerializeField] float m_cooldownTime = 5f;
    [SerializeField] float m_delay = 0.25f;
    [SerializeField] float m_triggerWidth = 0.5f;

    [SerializeField] int m_projectileDamage;
    [SerializeField] float m_projectileKnockback;

    [Space]
    [SerializeField] Sprite m_idleSprite;
    [SerializeField] Sprite m_preparedSprite;

    [SerializeField] Projectile m_projectile;

    [Space]
    [SerializeField] SpriteRenderer m_renderer;

    [Inject] readonly private PlayerManager PlayerManager;

    private float m_launchTimer = 0f;
    private List<Projectile> m_activeProjectiles = new List<Projectile>();

    #region iRoomObject Implementation
    public bool IsEnabled { get; private set; }
    public bool PersistantRespawn => true;
    public Action<IRoomObject> OnDestroy { get; set; }

    public void EnableObject()
    {
        IsEnabled = true;
        m_launchTimer = m_cooldownTime;
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
        m_launchTimer = m_cooldownTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsEnabled) return;

        if(m_launchTimer > m_cooldownTime)
        {
            m_renderer.sprite = m_preparedSprite;

            if (m_launchTimer > m_cooldownTime + m_delay && IsPlayerInFront())
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

        }
        else
        {
            m_renderer.sprite = m_idleSprite;
        }

        m_launchTimer += Time.deltaTime;
    }

    private bool IsPlayerInFront()
    {
        Vector3 toOther = PlayerManager.Player.transform.position - transform.position;
        float dotValue = Vector3.Dot(transform.right * transform.lossyScale.x, toOther);
        float orthogonalValue = MathF.Sqrt(toOther.sqrMagnitude - (dotValue * dotValue));

        return dotValue > 0 && orthogonalValue < m_triggerWidth;
    }

    private void ClearProjectile(Projectile projectile)
    {
        m_activeProjectiles.Remove(projectile);
    }
}
