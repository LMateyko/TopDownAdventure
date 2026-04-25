using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Sprite m_fullHeartSprite;
    [SerializeField] private Sprite m_emptyHeartSprite;

    [SerializeField] private Image[] m_playerHearts;
    [SerializeField] private TMP_Text m_keyText;
    [SerializeField] private TMP_Text m_coinText;

    private PlayerController m_playerToMove;

    private void Awake()
    {
        m_playerToMove = FindFirstObjectByType<PlayerController>();
        m_playerToMove.HealthChanged += OnHeathChanged;
        m_playerToMove.KeysChanged += OnKeyChanged;
        m_playerToMove.CoinsChanged += OnCoinsChanged;
    }

    private void OnHeathChanged(int maxHealth, int currentHealth, int newHealth)
    {
        for(int i = 0; i < m_playerHearts.Length; i++)
        {
            if (i >= maxHealth)
                m_playerHearts[i].gameObject.SetActive(false);
            else
            {
                m_playerHearts[i].gameObject.SetActive(true);
                if (i >= newHealth)
                    m_playerHearts[i].sprite = m_emptyHeartSprite;
                else
                    m_playerHearts[i].sprite = m_fullHeartSprite;
            }
        }
    }

    private void OnKeyChanged(int newValue)
    {
        m_keyText.text = " x " + newValue;
    }
    private void OnCoinsChanged(int newValue)
    {
        m_coinText.text = " x " + newValue;
    }
}
