using Reflex.Attributes;
using Reflex.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputUI : MonoBehaviour
{
    [Serializable]
    public struct InputDisplay
    {
        public WeaponConfiguration.WeaponEnum weaponKey;
        public GameObject enabledDisplay;
        public GameObject disabledDisplay;
    }

    [SerializeField] private InputDisplay[] m_inputDisplays;

    [Inject] readonly private PlayerInventory PlayerInventory;

    Dictionary<WeaponConfiguration.WeaponEnum, (GameObject, GameObject)> m_weaponEnabledDisplays = new Dictionary<WeaponConfiguration.WeaponEnum, (GameObject, GameObject)> ();

    private void Awake()
    {
        foreach (var input in m_inputDisplays)
        {
            m_weaponEnabledDisplays.Add(input.weaponKey, (input.enabledDisplay, input.disabledDisplay));
            HideWeapon(input.weaponKey);
        }
    }

    private void Start()
    {
        Reflex.Injectors.GameObjectInjector.InjectObject(gameObject, Container.RootContainer);

        // Configure Player Inventory Events
        PlayerInventory.OnWeaponAdded += RevealWeapon;

        PlayerInventory.LoadInitialWeapons();
    }

    private void HideWeapon(WeaponConfiguration.WeaponEnum newWeapon)
    {
        m_weaponEnabledDisplays[newWeapon].Item1.SetActive(false);
        m_weaponEnabledDisplays[newWeapon].Item2.SetActive(true);
    }

    private void RevealWeapon(WeaponConfiguration.WeaponEnum newWeapon)
    {
        m_weaponEnabledDisplays[newWeapon].Item1.SetActive(true);
        m_weaponEnabledDisplays[newWeapon].Item2.SetActive(false);
    }
}
