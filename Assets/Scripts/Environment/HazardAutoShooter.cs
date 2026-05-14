using System;
using System.Collections.Generic;
using UnityEngine;

public class HazardAutoShooter : ProjectileHazard
{
    [Header("Auto Fire Settings")]
    [SerializeField] float m_idleTime = 5f;
    [SerializeField] float m_preparedTime = .25f;
    [SerializeField] float m_timeOffset = 0f;

    protected override void SetDefaultTime()
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
            ShootProjectile();
            m_launchTimer = 0;
            m_renderer.sprite = m_idleSprite;
        }

        m_launchTimer += Time.deltaTime;
    }
    
}
