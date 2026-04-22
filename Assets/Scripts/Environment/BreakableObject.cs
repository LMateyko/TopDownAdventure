using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private List<WeaponConfiguration.WeaponEnum> m_effectiveWeapons;

    [SerializeField] private string m_animationPrefix;
    [SerializeField] private Animator m_animator;

    protected virtual void DestroyObject()
    {
        StartCoroutine(BreakCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only process triggers that are attacks
        if (collision.gameObject.layer != LayerMask.NameToLayer("Attack"))
            return;

        PlayerController attacker = collision.attachedRigidbody.GetComponent<PlayerController>();
        if (attacker != null && m_effectiveWeapons.Contains(attacker.CurrentWeapon))
            DestroyObject();
    }

    private IEnumerator BreakCoroutine()
    {
        m_animator.Play($"{m_animationPrefix}_Break");

        yield return null;

        while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        Destroy(gameObject);
    }
}
