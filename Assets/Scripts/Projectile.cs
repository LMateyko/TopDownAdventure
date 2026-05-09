using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour, IDamager
{
    [SerializeField] private float m_speed = 5f;
    [SerializeField] private SpriteRenderer m_renderer;

    private Vector3 TravelVector => transform.localScale.x * transform.right * m_speed * Time.deltaTime;

    #region IDamager
    public int Damage { get; private set; } 

    public float KnockbackForce { get; private set; }

    public bool AttackEnabled => true;

    public void DamageTarget(IDamageable defender)
    {
        if (!AttackEnabled) return;

        defender.TakeDamage(Damage);
        defender.Knockback(transform.right, force: KnockbackForce);
    }

    #endregion

    public void SetAttackData(int damage, float knockback)
    {
        Damage = damage;
        KnockbackForce = knockback;
    }

    private void Update()
    {
        transform.position += TravelVector;

        if (!IsOnScreen())
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

    private bool IsOnScreen()
    {
        var camera = Camera.main;
        var maxViewportPoint = camera.WorldToViewportPoint(m_renderer.bounds.max);
        var minViewportPoint = camera.WorldToViewportPoint(m_renderer.bounds.min);

        bool renderOnScreen = (maxViewportPoint.x > 0f && minViewportPoint.x < 1f &&
                                    maxViewportPoint.y > 0f && minViewportPoint.y < 1f);
        return renderOnScreen;
    }

    
}
