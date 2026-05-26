using Reflex.Attributes;
using System.Collections;
using UnityEngine;

public class HazardCrackedPit : MonoBehaviour
{
    [SerializeField] AudioClip m_crackOpenAudio;
    [Tooltip("Time between crack sprite transitions")]
    [SerializeField] float m_timePerState;
    [Tooltip("How many animations while opening, including open")]
    [SerializeField] int m_crackedStates = 3;
    [SerializeField] Vector2 m_openPitSize = new Vector2(0.25f, 0.25f);
    [SerializeField] Animator m_pitAnimator;

    [Inject] readonly private AudioManager AudioManager;

    private int m_crackIndex = 0;
    private Coroutine m_crackCountRoutine;

    private void Start()
    {
        m_pitAnimator.Play("Pit_Crack_0");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_crackIndex >= m_crackedStates)
            return;

        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            m_crackCountRoutine = StartCoroutine(CoCrackCountdown(foundPlayer));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_crackIndex >= m_crackedStates)
            return;

        var foundPlayer = collision.attachedRigidbody.gameObject.GetComponent<PlayerController>();
        if (foundPlayer && collision.CompareTag("Player"))
        {
            if (m_crackCountRoutine != null)
                StopCoroutine(m_crackCountRoutine);
        }
    }

    private IEnumerator CoCrackCountdown(PlayerController player)
    {
        while (m_crackIndex < m_crackedStates - 1)
        {
            m_pitAnimator.Play($"Pit_Crack_{m_crackIndex}");
            yield return new WaitForSeconds(m_timePerState);

            m_crackIndex++;
        }

        AudioManager.PlaySfxAtLocation(m_crackOpenAudio, transform.position);
        m_pitAnimator.Play($"Pit_Crack_Open");
        var collider = GetComponent<BoxCollider2D>();
        collider.size = m_openPitSize;

        HazardPit hazardPit = gameObject.AddComponent<HazardPit>();

        if(player is IDamageable damageable)
            hazardPit.DamageTarget(damageable);

        player.transform.position = transform.position;
        player.FallIntoPit();

        m_crackIndex = m_crackedStates;
    }
}
