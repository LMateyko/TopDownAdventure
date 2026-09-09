using Reflex.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputUI : MonoBehaviour
{
    [Serializable]
    public struct InputDisplay
    {
        public WeaponConfiguration.WeaponEnum weaponKey;
        public AbilityPanelUI enabledAbilityPanel;
        public AbilityPanelUI disabledAbilityPanel;
    }

    [SerializeField] private InputDisplay[] m_inputDisplays;
    [SerializeField] private AbilityPanelUI m_movementDisplay;

    Dictionary<WeaponConfiguration.WeaponEnum, (AbilityPanelUI, AbilityPanelUI)> m_weaponEnabledDisplays = new Dictionary<WeaponConfiguration.WeaponEnum, (AbilityPanelUI, AbilityPanelUI)> ();

    public void SetInventoryEvents(PlayerInventory inventory)
    {
        inventory.OnWeaponAdded += RevealWeapon;
    }

    private void Awake()
    {
        foreach (var input in m_inputDisplays)
        {
            m_weaponEnabledDisplays.Add(input.weaponKey, (input.enabledAbilityPanel, input.disabledAbilityPanel));
            HideWeapon(input.weaponKey);
        }

        InputSystem.onActionChange += InputSystem_onActionChange;
    }

    private void InputSystem_onActionChange(object inputActionObject, InputActionChange inputActionChange)
    {
        InputAction inputAction = inputActionObject as InputAction;
        if(inputActionChange == InputActionChange.ActionStarted)
        {
            var deviceType = inputAction.activeControl.device.displayName.ToLower();
            bool isKeyboard = deviceType.Contains("keyboard") || deviceType.Contains("mouse");
            foreach (var input in m_inputDisplays)
            {
                input.enabledAbilityPanel.SetControlDisplay(isKeyboard);
                input.disabledAbilityPanel.SetControlDisplay(isKeyboard);
            }

            m_movementDisplay.SetControlDisplay(isKeyboard);
        }   
    }

    private void HideWeapon(WeaponConfiguration.WeaponEnum newWeapon)
    {
        m_weaponEnabledDisplays[newWeapon].Item1.gameObject.SetActive(false);
        m_weaponEnabledDisplays[newWeapon].Item2.gameObject.SetActive(true);
    }

    private void RevealWeapon(WeaponConfiguration.WeaponEnum newWeapon)
    {
        m_weaponEnabledDisplays[newWeapon].Item1.gameObject.SetActive(true);
        m_weaponEnabledDisplays[newWeapon].Item2.gameObject.SetActive(false);
    }
}
