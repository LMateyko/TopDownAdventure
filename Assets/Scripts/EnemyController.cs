using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float m_speed = 5f;
    [SerializeField] private int m_maxHealth = 3;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody;

    private bool IsAlive => m_currentHealth > 0;

    private Vector2 m_moveDirection = Vector2.right;
    private int m_currentHealth;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);
    private readonly float WallOffset = .025f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_animator.Play("Slime_Run");
        m_currentHealth = m_maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsAlive)
        {
            m_rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        m_rigidbody.linearVelocity = m_moveDirection * m_speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive)
            return;

        ChangeDirection(collision.contacts);
    }

    private void ChangeDirection(ContactPoint2D[] currentContacts)
    {
        // Determine valid directions based on current contacts 
        List<Vector2> directions = new List<Vector2>() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector3 cleanNormal;
        foreach (var contact in currentContacts)
        {
            cleanNormal = contact.normal;
            if (Mathf.Abs(cleanNormal.y) < 0.1f)
                cleanNormal.y = 0;
            if (Mathf.Abs(cleanNormal.x) < 0.1f)
                cleanNormal.x = 0;

            directions.Remove(cleanNormal * -1);

            // ofset position slightly by contact normals
            transform.position += cleanNormal * WallOffset;
        }

        var randomIndex = Random.Range(0, directions.Count);
        m_moveDirection = directions[randomIndex];

        // Adjust facing based on new movement direction
        if (m_moveDirection.x > 0)
            transform.localScale = FaceRightScale;
        else if (m_moveDirection.x < 0)
            transform.localScale = FaceLeftScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAlive)
            return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Attack"))
            TakeDamage();
    }

    private void TakeDamage()
    {
        m_currentHealth--;

        if (m_currentHealth <= 0)
            Destroy(gameObject);
        else
            m_animator.Play("Slime_Hurt");
    }
    
}
