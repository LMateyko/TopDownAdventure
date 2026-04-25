using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController, InputSystem_Player.IPlayerActions
{
    [Header("Weapon Configurations")]
    [SerializeField] private Animator m_weaponAnimator;
    [SerializeField] private WeaponConfiguration m_swordConfig;
    [SerializeField] private WeaponConfiguration m_bookConfig;
    [SerializeField] private WeaponConfiguration m_pickConfig;
    [SerializeField] private WeaponConfiguration m_bowConfig;

    [Header("Weapon Sockets")]
    [SerializeField] private Transform m_socketUpSwing;
    [SerializeField] private Transform m_socketForwardSwing;
    [SerializeField] private Transform m_socketDownSwing;

    public Action<int, int, int> HealthChanged;

    public override int Damage => m_weaponMap[CurrentWeapon].WeaponDamage;
    public override float KnockbackForce => m_weaponMap[CurrentWeapon].WeaponKnockback;
    public WeaponConfiguration.WeaponEnum CurrentWeapon { get; private set; } = WeaponConfiguration.WeaponEnum.None;

    private InputSystem_Player m_playerInputSystem;
    private InputSystem_Player.PlayerActions m_playerActions;

    private bool m_weaponAnimStarted = false;
    private Dictionary<WeaponConfiguration.WeaponEnum, WeaponConfiguration> m_weaponMap 
        = new Dictionary<WeaponConfiguration.WeaponEnum, WeaponConfiguration>();

    private Vector2 m_targetVelocity;

    /// <summary>
    /// Disable player Input for external service ex: dialog. 
    /// Re-enable with <see cref="ReEnableInput"/>
    /// </summary>
    public void DisableInputForExternalInteraction()
    {
        // TODO: If this ends up being called from multiple directions, increment count of disabled sources.
        m_playerActions.Disable();
    }

    /// <summary>
    /// Re-enable player input disabled through <see cref="DisableInputForExternalInteraction"/>
    /// </summary>
    public void ReEnableInput()
    {
        // TODO: If this ends up being called from multiple directions, decrement count of disabled sources.
        m_playerActions.Enable();
    }

    #region Interactables
    private Interactable m_currentInteractable;

    public void PrepareInteraction(Interactable newInteractable)
    {
        m_currentInteractable = newInteractable;
    }

    public void ClearInteraction(Interactable newInteractable)
    {
        if (m_currentInteractable == newInteractable)
            m_currentInteractable = null;
    }

    private bool TriggerInteraction()
    {
        if (m_currentInteractable != null && m_currentInteractable.gameObject.activeInHierarchy)
        {
            m_currentInteractable.TriggerInteraction();
            return true;
        }
        else
            return false; 
    }

    #endregion

    #region IPlayerActions Implementation
    public void OnMove(InputAction.CallbackContext context)
    {
        var moveValue = context.ReadValue<Vector2>();
        m_targetVelocity = moveValue * m_speed;
    }

    public void OnSword(InputAction.CallbackContext context)
    {
        if (TriggerInteraction()) return;

        if(context.started)
            UseWeapon(m_swordConfig);
    }

    public void OnBookBlock(InputAction.CallbackContext context)
    {
        if (TriggerInteraction()) return;

        if (context.started)
        {
            UseWeapon(m_bookConfig);
        }
        else if (context.canceled)
        {
            StopWeapon();
        }
    }

    public void OnBowShoot(InputAction.CallbackContext context)
    {
        if (TriggerInteraction()) return;

        if (context.started)
            UseWeapon(m_bowConfig);
    }

    public void OnPickSwing(InputAction.CallbackContext context)
    {
        if (TriggerInteraction()) return;

        if (context.started)
            UseWeapon(m_pickConfig);
    }

    #endregion

    #region Unity Functions
    private void Awake()
    {
        m_playerInputSystem = new InputSystem_Player();
        m_playerActions = m_playerInputSystem.Player;
        m_playerActions.AddCallbacks(this);

        m_weaponMap.Add(WeaponConfiguration.WeaponEnum.Sword,   m_swordConfig);
        m_weaponMap.Add(WeaponConfiguration.WeaponEnum.Book,    m_bookConfig);
        m_weaponMap.Add(WeaponConfiguration.WeaponEnum.Pick,    m_pickConfig);
        m_weaponMap.Add(WeaponConfiguration.WeaponEnum.Bow,     m_bowConfig);
    }

    protected override void Start()
    {
        base.Start();

        HealthChanged?.Invoke(m_maxHealth, m_currentHealth, m_currentHealth);
    }

    private void OnEnable()
    {
        m_playerActions.Enable();
    }

    private void OnDisable()
    {
        m_playerActions.Disable();
    }

    private void OnDestroy()
    {
        m_playerInputSystem.Dispose();
    }

    protected override void Update()
    {
        base.Update();

        if (!IsAlive)
        {
            SetVelocity(Vector2.zero, false);

            // TODO: Return to checkpoint on death
            if (IsAnimPlaying("Death") && IsAnimComplete())
                Destroy(gameObject);

            return;
        }

        if (IsAnimPlaying("Hurt"))
            return;

        if (CurrentWeapon == WeaponConfiguration.WeaponEnum.None)
        {
            SetVelocity(m_targetVelocity, true);
        }
        else
        {
            SetVelocity(m_targetVelocity * m_weaponMap[CurrentWeapon].SpeedMultiplier, 
                setFacing: m_weaponMap[CurrentWeapon].AllowFacingChange);

            if (IsInWeaponAnim(m_weaponMap[CurrentWeapon].WeaponAnimation))
                m_weaponAnimStarted = true;

            if (m_weaponMap[CurrentWeapon].AnimationTransition && m_weaponAnimStarted
                && !IsInWeaponAnim(m_weaponMap[CurrentWeapon].WeaponAnimation))
            {
                CurrentWeapon = WeaponConfiguration.WeaponEnum.None;
            }
        }

        if(!IsAnimPlaying("Hurt"))
        {
            if (m_targetVelocity == Vector2.zero)
                PlayAnimation("Idle");
            else
                PlayAnimation("Run");
        }
    }
    #endregion

    #region Character Overrides

    public override void HealCharacter(int heal)
    {
        HealthChanged?.Invoke(m_maxHealth, m_currentHealth, m_currentHealth + heal);

        base.HealCharacter(heal);
    }

    protected override void TakeDamage(int damage)
    {
        HealthChanged?.Invoke(m_maxHealth, m_currentHealth, m_currentHealth - damage);

        base.TakeDamage(damage);
    }

    protected override void KillCharacter()
    {
        PlayAnimation("Death");
    }
    
    #endregion

    #region Weapon
    private void UseWeapon(WeaponConfiguration currentWeapon)
    {
        m_weaponAnimStarted = false;
        CurrentWeapon = currentWeapon.WeaponType;
        m_weaponAnimator.Play(currentWeapon.WeaponAnimation);
    }

    private void StopWeapon()
    {
        m_weaponAnimator.Play("Weapon_Idle");
        CurrentWeapon = WeaponConfiguration.WeaponEnum.None;
    }

    public override void SetFacing(Vector2 moveValue)
    {
        base.SetFacing(moveValue);

        if (moveValue.y > 0)
            m_weaponAnimator.transform.SetParent(m_socketUpSwing, false);
        else if (moveValue.y < 0)
            m_weaponAnimator.transform.SetParent(m_socketDownSwing, false);
        else if (moveValue.y == 0 && moveValue.x != 0)
            m_weaponAnimator.transform.SetParent(m_socketForwardSwing, false);
    }

    private bool IsInWeaponAnim(string weaponAnim)
    {
        var animHash = Animator.StringToHash(weaponAnim);
        return m_weaponAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == animHash;
    }
    #endregion
}
