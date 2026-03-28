using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : CharacterController
{
    private Vector2 m_moveDirection = Vector2.right;
    private readonly float WallOffset = .025f;

    protected override void Start()
    {
        base.Start();
        PlayAnimation("Run");
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

        SetRandomDirection(collision.contacts);
    }

    private void SetRandomDirection(ContactPoint2D[] currentContacts)
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

        SetFacing(m_moveDirection);
    }
}
