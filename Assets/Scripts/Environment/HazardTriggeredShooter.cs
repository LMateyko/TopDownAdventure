using Reflex.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HazardTriggeredShooter : ProjectileHazard
{
    [Header("Triggered Fire Settings")]
    [SerializeField] float m_cooldownTime = 5f;
    [SerializeField] float m_delay = 0.25f;
    [SerializeField] float m_triggerWidth = 0.5f;

    [Inject] readonly private PlayerManager PlayerManager;

    protected override void SetDefaultTime()
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
                ShootProjectile();

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
}
