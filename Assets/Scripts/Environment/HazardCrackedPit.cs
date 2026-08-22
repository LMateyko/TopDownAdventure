using Reflex.Attributes;
using System.Collections;
using System.Collections.Generic;
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
    private List<BaseCharacterController> m_detectedCharacters = new List<BaseCharacterController>();

    private void Start()
    {
        m_pitAnimator.Play("Pit_Crack_0");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_crackIndex >= m_crackedStates)
            return;

        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter && !foundCharacter.IsFloating)
        {
            m_detectedCharacters.Add(foundCharacter);

            // Trigger for enemies immedietely 
            if (collision.CompareTag("Player"))
                m_crackCountRoutine = StartCoroutine(CoCrackCountdown(foundCharacter));
            else
                CrackPit();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_crackIndex >= m_crackedStates)
            return;

        var foundCharacter = collision.attachedRigidbody.gameObject.GetComponent<BaseCharacterController>();
        if (foundCharacter)
        {
            m_detectedCharacters.Remove(foundCharacter);

            if (collision.CompareTag("Player") && m_crackCountRoutine != null)
                StopCoroutine(m_crackCountRoutine);
        }
    }

    private IEnumerator CoCrackCountdown(BaseCharacterController player)
    {
        while (m_crackIndex < m_crackedStates - 1)
        {
            m_pitAnimator.Play($"Pit_Crack_{m_crackIndex}");
            yield return new WaitForSeconds(m_timePerState);

            m_crackIndex++;
        }

        CrackPit();
        
        m_crackCountRoutine = null;
    }

    private void CrackPit()
    {
        AudioManager.PlaySfxAtLocation(m_crackOpenAudio, transform.position);
        m_pitAnimator.Play($"Pit_Crack_Open");
        var collider = GetComponent<BoxCollider2D>();
        collider.size = m_openPitSize;

        HazardPit hazardPit = gameObject.AddComponent<HazardPit>();

        foreach(var character in m_detectedCharacters)
        {
            if (character is IDamageable damageable)
                hazardPit.DamageTarget(damageable);

            character.transform.position = transform.position;
            character.FallIntoPit();
        }

        m_crackIndex = m_crackedStates;
    }
}
