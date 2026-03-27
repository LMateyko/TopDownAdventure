using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float m_speed = 5f;

    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody2D m_rigidbody;

    private Vector2 m_moveDirection = Vector2.right;
    private int m_lastContactTotal = 0;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);
    private readonly float WallOffset = .025f;

    private ContactPoint2D[] m_contactCache = new ContactPoint2D[8];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_animator.Play("Slime_Run");
    }

    // Update is called once per frame
    private void Update()
    {
        m_rigidbody.linearVelocity = m_moveDirection * m_speed;

        //EvaluateContacts();
    }

    private void EvaluateContacts()
    {
        var totalContacts = m_rigidbody.GetContacts(m_contactCache);

        // No Need to Update State
        if (totalContacts == m_lastContactTotal)
            return;

        ChangeDirection(m_contactCache);

        m_lastContactTotal = totalContacts;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeDirection(collision.contacts);
    }

    private void ChangeDirection(ContactPoint2D[] currentContacts)
    {
        // Determine valid directions based on current contacts 
        List<Vector2> directions = new List<Vector2>() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Vector3 cleanNormal;
        foreach(var contact in currentContacts)
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
}
