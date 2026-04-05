using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Sprite m_fullHeartSprite;
    [SerializeField] private Sprite m_emptyHeartSprite;

    [SerializeField] private Image[] playerHearts;

    private PlayerController m_playerToMove;

    private void Awake()
    {
        m_playerToMove = FindFirstObjectByType<PlayerController>();
        m_playerToMove.HealthChanged += OnHeathChanged;
    }

    private void OnHeathChanged(int maxHealth, int currentHealth, int newHealth)
    {
        for(int i = 0; i < playerHearts.Length; i++)
        {
            if (i >= maxHealth)
                playerHearts[i].gameObject.SetActive(false);
            else
            {
                playerHearts[i].gameObject.SetActive(true);
                if (i >= newHealth)
                    playerHearts[i].sprite = m_emptyHeartSprite;
                else
                    playerHearts[i].sprite = m_fullHeartSprite;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
