using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IRoomObject
{
    [SerializeField] private List<WeaponConfiguration.WeaponEnum> m_effectiveWeapons;
    [SerializeField] private bool m_respawn = false;

    [SerializeField] private string m_animationPrefix;
    [SerializeField] private Animator m_animator;

    #region IRoomObject
    public bool IsEnabled { get; private set; }
    public bool PersistantRespawn => m_respawn;

    public Action<IRoomObject> OnDestroy { get; set; }

    public void EnableObject()
    {
        IsEnabled = true;
        m_animator.Play($"{m_animationPrefix}_Idle");
    }

    public void DisableObject()
    {
        IsEnabled = false;
        m_animator.Play($"{m_animationPrefix}_Disable");
    }
    #endregion

    protected virtual void DestroyObject()
    {
        IsEnabled = false;
        StartCoroutine(BreakCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsEnabled) return;

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

        if (PersistantRespawn)
            DisableObject();
        else
        {
            OnDestroy?.Invoke(this);
            Destroy(gameObject);
        }   
    }
}
