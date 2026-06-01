using System;
using UnityEngine;

public class PlayerInventory
{
    public Action<int> KeysChanged;
    public Action<int> CoinsChanged;

    private int m_keys = 0;
    private int m_coins = 0;

    public int Keys
    {
        get => m_keys;
        set
        {
            KeysChanged?.Invoke(value);
            m_keys = value;
        }
    }

    public int Coins
    {
        get => m_coins;
        set
        {
            CoinsChanged?.Invoke(value);
            m_coins = value;
        }
    }

    /// <summary>
    /// Load inventory from data and invoke UI Events
    /// </summary>
    public void LoadInitialValues()
    {
        CoinsChanged?.Invoke(m_coins);
        KeysChanged?.Invoke(m_keys);
    }
}
