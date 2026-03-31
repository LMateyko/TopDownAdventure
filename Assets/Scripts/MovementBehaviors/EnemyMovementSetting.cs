using UnityEngine;

public abstract class EnemyMovementSetting : MonoBehaviour
{
    [SerializeField] protected EnemyController m_enemy;

    public abstract void InitializeMovement();

    public abstract void RestartMovement();

    public abstract void OnUpdate();

    public abstract void OnCollision(Collision2D collision);
}
