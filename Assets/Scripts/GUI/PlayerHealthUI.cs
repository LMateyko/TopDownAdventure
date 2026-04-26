using Reflex.Attributes;
using System;
using System.Diagnostics;
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

    [Inject] readonly private PlayerManager PlayerManager;

    private PlayerController m_playerToMove;

    private void Awake()
    {
        //UnityEngine.Debug.Log("Enter UI");

        //TODO: Resolve issue null issue when using player manager
        m_playerToMove = FindFirstObjectByType<PlayerController>();
        m_playerToMove.HealthChanged += OnHeathChanged;
        m_playerToMove.KeysChanged += OnKeyChanged;
        m_playerToMove.CoinsChanged += OnCoinsChanged;

        //PlayerManager.Player.HealthChanged += OnHeathChanged;
        //PlayerManager.Player.KeysChanged += OnKeyChanged;
        //PlayerManager.Player.CoinsChanged += OnCoinsChanged;
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
