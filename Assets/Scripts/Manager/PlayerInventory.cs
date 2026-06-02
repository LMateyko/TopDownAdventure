using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory
{
    public Action<int> OnKeysChanged;
    public Action<int> OnCoinsChanged;

    public Action<WeaponConfiguration.WeaponEnum> OnWeaponAdded;

    private int m_keys = 0;
    private int m_coins = 0;

    private List<WeaponConfiguration.WeaponEnum> m_weaponInventory = new List<WeaponConfiguration.WeaponEnum>();

    public int Keys
    {
        get => m_keys;
        set
        {
            OnKeysChanged?.Invoke(value);
            m_keys = value;
        }
    }

    public int Coins
    {
        get => m_coins;
        set
        {
            OnCoinsChanged?.Invoke(value);
            m_coins = value;
        }
    }

    /// <summary>
    /// Load inventory from data and invoke UI Events
    /// </summary>
    public void LoadInitialValues()
    {
        AddWeaponToInventory(WeaponConfiguration.WeaponEnum.Sword);
        AddWeaponToInventory(WeaponConfiguration.WeaponEnum.Book);

        Coins = 0;
        Keys = 0;
    }

    /// <summary>
    /// Check a weapon to see if it can currently be used
    /// </summary>
    /// <param name="checkedWeapon"></param>
    /// <returns>True if the weapon is currently within the player's inventory</returns>
    public bool WeaponAvailable(WeaponConfiguration.WeaponEnum checkedWeapon)
    {
        return m_weaponInventory.Contains(checkedWeapon);
    }

    /// <summary>
    /// Add Weapon to inventory via chest or other source
    /// </summary>
    /// <param name="newWeapon"></param>
    public void AddWeaponToInventory(WeaponConfiguration.WeaponEnum newWeapon)
    {
        if(!m_weaponInventory.Contains(newWeapon))
        {
            m_weaponInventory.Add(newWeapon);
            OnWeaponAdded?.Invoke(newWeapon);
        }
    }
}
