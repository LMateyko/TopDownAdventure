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
    [SerializeField, ConditionalHide("m_pausePlayerForRewards")] private float m_playerPauseDelay = 0.25f;
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
        int i = 0;

        if (m_pausePlayerForRewards)
        {
            PlayerManager.PausePlayer();
            yield return new WaitForSeconds(m_playerPauseDelay);
        }

        yield return new WaitForEndOfFrame();

        while (i < m_rewardTriggers.Count)
        {
            if (m_rewardTriggers[i].delay < 0)
                yield return new WaitForSeconds(m_rewardTriggers[i].delay);

            m_rewardTriggers[i].triggeredEvent?.Invoke();
            i++;

            yield return null;
        }

        if (m_pausePlayerForRewards)
        {
            yield return new WaitForSeconds(m_playerPauseDelay);
            PlayerManager.ResumePlayer();
        }

        Destroy(gameObject);
    }
}
