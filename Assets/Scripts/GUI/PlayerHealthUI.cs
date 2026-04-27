using Reflex.Attributes;
using Reflex.Core;
using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Sprite m_fullHeartSprite;
    [SerializeField] private Sprite m_emptyHeartSprite;

    [SerializeField] private Image[] m_playerHearts;
    [SerializeField] private TMP_Text m_keyText;
    [SerializeField] private TMP_Text m_coinText;

    public void SetPlayerEvents(PlayerController player)
    {
        player.HealthChanged += OnHeathChanged;
        player.KeysChanged += OnKeyChanged;
        player.CoinsChanged += OnCoinsChanged;
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
