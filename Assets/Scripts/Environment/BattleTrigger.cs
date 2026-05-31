using Reflex.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleTrigger : MonoBehaviour
{
    [Serializable]
    public struct BattleRewardEvent
    {
        public string label;
        public float delay;
        public UnityEvent triggeredEvent;
    }

    [SerializeField] private bool m_pausePlayerForRewards = false;
    [SerializeField] private AudioClip m_rewardAudio;
    [SerializeField] private EnemyController[] m_foughtEnemies;
    [SerializeField] private List<BattleRewardEvent> m_rewardTriggers;

    [Inject] readonly private PlayerManager PlayerManager;
    [Inject] readonly private AudioManager AudioManager;

    private int m_enemiesRemaining = 0;

    public void PlayFanfare()
    {
        AudioManager.PlaySfxAtLocation(m_rewardAudio, transform.position);
    }

    private void Start()
    {
        m_enemiesRemaining = m_foughtEnemies.Length;
        foreach (var enemy in m_foughtEnemies)
        {
            enemy.OnKillCharacter += CheckTrigger;
        }
    }

    private void CheckTrigger(BaseCharacterController controller)
    {
        m_enemiesRemaining--;
        if (m_enemiesRemaining <= 0)
        {
            StartCoroutine(ResolveEventsRoutine());
        }
    }

    private IEnumerator ResolveEventsRoutine()
    {
        if(m_pausePlayerForRewards)
            PlayerManager.PausePlayer();

        int i = 0;
        float delayTimer = 0;

        yield return new WaitForEndOfFrame();

        while (i < m_rewardTriggers.Count)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= m_rewardTriggers[i].delay)
            {
                m_rewardTriggers[i].triggeredEvent?.Invoke();
                delayTimer = 0;
                i++;
            }

            yield return null;
        }

        if (m_pausePlayerForRewards)
            PlayerManager.ResumePlayer();

        Destroy(gameObject);
    }
}
