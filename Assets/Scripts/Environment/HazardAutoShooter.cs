using UnityEngine;

public class HazardAutoShooter : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_launchTimer = m_timeOffset;
    }

    // Update is called once per frame
    void Update()
    {
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

            m_launchTimer = 0;
            m_renderer.sprite = m_idleSprite;
        }

        m_launchTimer += Time.deltaTime;
    }
}
