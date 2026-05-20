using UnityEngine;

[RequireComponent (typeof(Animator))]
public class SpriteParticle : MonoBehaviour
{
    Animator m_animator;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (m_animator != null)
        {
            if(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                // TODO: Return to Pool
                Destroy(gameObject);
            }
        }
    }
}
