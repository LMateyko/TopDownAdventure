using System.Collections.Generic;
using UnityEngine;

public class EnemyWanderMovement : EnemyMovementSetting
{
    private Vector2 m_moveDirection = Vector2.right;
    private readonly float WallOffset = .025f;

    public override void InitializeMovement() {}

    public override void RestartMovement()
    {
        SetRandomDirection(m_enemy);
    }

    public override void OnUpdate()
    {
        m_enemy.SetVelocity(m_moveDirection * m_enemy.CurrentSpeed, true);
    }

    public override void OnCollision(Collision2D collision)
    {
        SetRandomDirection(m_enemy, collision.contacts);
    }

    private void SetRandomDirection(EnemyController enemy)
    {
        List<Vector2> directions = new List<Vector2>() { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        var randomIndex = Random.Range(0, directions.Count);
        m_moveDirection = directions[randomIndex];

        enemy.SetFacing(m_moveDirection);
    }

    private void SetRandomDirection(EnemyController enemy, ContactPoint2D[] currentContacts)
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
            enemy.transform.position += cleanNormal * WallOffset;
        }

        var randomIndex = Random.Range(0, directions.Count);
        m_moveDirection = directions[randomIndex];

        enemy.SetFacing(m_moveDirection);
    }
}
