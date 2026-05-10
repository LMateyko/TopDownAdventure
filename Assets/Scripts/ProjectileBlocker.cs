using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ProjectileBlocker : MonoBehaviour
{
    [SerializeField] private UnityEvent<Projectile> OnProjectileBlocked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.GetComponent<Projectile>();
        if(projectile != null)
        {
            OnProjectileBlocked?.Invoke(projectile);
        }
    }
}
