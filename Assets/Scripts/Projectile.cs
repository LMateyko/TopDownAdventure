using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour, IDamager
{
    [SerializeField] private float m_speed = 5f;

    [Space]
    [SerializeField] private SpriteRenderer m_renderer;

    public Action<Projectile> OnDestroy { get; set; }

    public Vector3 DirectionVector => transform.localScale.x * transform.right;
    private Vector3 TravelVector => DirectionVector * m_speed * Time.deltaTime;

    private Transform m_projectileOwner = null;

    #region IDamager
    public int Damage { get; private set; } 

    public float KnockbackForce { get; private set; }

    public bool AttackEnabled => true;

    public bool DamageTarget(IDamageable defender)
    {
        if (!AttackEnabled) return false;
        if (!defender.IsValidTarget()) return false;

        defender.Knockback(DirectionVector, force: KnockbackForce);
        defender.TakeDamage(this, Damage);

        return true;
    }

    #endregion

    public void SetOwner(Transform owner)
    {
        m_projectileOwner = owner;
    }

    public void SetLaunchVelocity(Vector2 velocity)
    {
        transform.right = velocity.normalized;
    }

    public void RotateToTransform(Transform parentTransform)
    {
        transform.position = parentTransform.position;
        transform.localScale = parentTransform.lossyScale;
        transform.rotation = parentTransform.rotation;
    }

    public void SetAttackData(int damage, float knockback)
    {
        Damage = damage;
        KnockbackForce = knockback;
    }

    private void Update()
    {
        transform.position += TravelVector;

        if (!IsOnScreen())
            DestroyProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundCharacter = collision.GetComponent<IDamageable>();
        // Check for valid component and prevent hitting damageable on the same team 
        if (foundCharacter != null && collision.CompareTag(gameObject.tag) == false)
        {
            if (!foundCharacter.IsValidTarget()) return;

            if (m_projectileOwner != null && foundCharacter.transform == m_projectileOwner)
                return;

            DamageTarget(foundCharacter);
        }

        // TODO: Display different effects based on the source of destruction
        // Hitting target vs terrain vs blocked

        DestroyProjectile();
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

    private void DestroyProjectile()
    {
        OnDestroy?.Invoke(this);
        Destroy(gameObject);
    }
    
}
