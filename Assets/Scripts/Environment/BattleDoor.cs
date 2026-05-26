using Reflex.Attributes;
using System;
using UnityEngine;

public class BattleDoor : MonoBehaviour
{
    [SerializeField] private AudioClip m_unlockAudio;
    [SerializeField] private EnemyController[] m_foughtEnemies;

    [Inject] readonly private AudioManager AudioManager;

    private int m_enemiesRemaining = 0;

    private void Start()
    {
        m_enemiesRemaining = m_foughtEnemies.Length;
        foreach(var enemy in m_foughtEnemies)
        {
            enemy.OnKillCharacter += CheckLock;
        }
    }

    private void CheckLock(BaseCharacterController controller)
    {
        m_enemiesRemaining--;
        if(m_enemiesRemaining <= 0)
        {
            if(m_unlockAudio != null)
                AudioManager.PlaySfxAtLocation(m_unlockAudio, transform.position);

            Destroy(gameObject);
        }
    }
}
