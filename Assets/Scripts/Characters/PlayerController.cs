using Reflex.Attributes;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController, InputSystem_Player.IPlayerActions
{
    [Header("Player Settings")]
    [SerializeField] private AnimationCurve m_transitionBlending;

    [Header("Weapon Configurations")]
    [SerializeField] private Animator m_weaponAnimator;
    [SerializeField] private BoxCollider2D m_weaponCollider;
    [SerializeField] private WeaponConfiguration m_swordConfig;
    [SerializeField] private WeaponConfiguration m_bookConfig;
    [SerializeField] private WeaponConfiguration m_pickConfig;
    [SerializeField] private WeaponConfiguration m_bowConfig;

    [Header("Weapon Sockets")]
    [SerializeField] private Transform m_socketUpSwing;
    [SerializeField] private Transform m_socketForwardSwing;
    [SerializeField] private Transform m_socketDownSwing;

    public Action OnFallComplete;
    public Action<int, int, int> OnHealthChanged;

    public override bool AttackEnabled => base.AttackEnabled && CurrentWeapon != WeaponConfiguration.WeaponEnum.None;
    public override int Damage => m_weaponMap[CurrentWeapon].WeaponDamage;
    public override float KnockbackForce => m_weaponMap[CurrentWeapon].WeaponKnockback;
    public WeaponConfiguration.WeaponEnum CurrentWeapon { get; private set; } = WeaponConfiguration.WeaponEnum.None;

    [Inject] readonly private PoolManager PoolManager;
    [Inject] readonly private PlayerInventory PlayerInventory;

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

    public override void FallIntoPit()
    {
        base.FallIntoPit();
        DisableInputForExternalInteraction();
    }

    #region Interactables
    private InteractableObject m_currentInteractable;

    public void PrepareInteraction(InteractableObject newInteractable)
    {
        m_currentInteractable = newInteractable;
    }

    public void ClearInteraction(InteractableObject newInteractable)
    {
        if (m_currentInteractable == newInteractable)
            m_currentInteractable = null;
    }

    private bool TriggerInteraction()
    {
        if (m_currentInteractable != null 
            && m_currentInteractable.gameObject.activeInHierarchy)
        {
            m_currentInteractable.TriggerInteraction(this);
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
        if (!PlayerInventory.WeaponAvailable(m_swordConfig.WeaponType)) return;

        if (TriggerInteraction()) return;

        if(context.started)
            UseWeapon(m_swordConfig);
    }

    public void OnBlockProjectile(Projectile projectile)
    {
        if(CurrentWeapon == WeaponConfiguration.WeaponEnum.Book)
        {
            m_weaponAnimator.Play("Weapon_Book_BlockPulse");
        }
        else
        {
            Debug.LogError("Non-Book weapon is blocking projectiles");
        }
    }

    public void OnBookBlock(InputAction.CallbackContext context)
    {
        if (!PlayerInventory.WeaponAvailable(m_bookConfig.WeaponType)) return;
        if (TriggerInteraction()) return;

        if (context.started)
        {
            UseWeapon(m_bookConfig);
            m_weaponCollider.gameObject.layer = LayerMask.NameToLayer("AttackBlocker");
        }
        else if (context.canceled)
        {
            m_weaponCollider.gameObject.layer = LayerMask.NameToLayer("Attack");
            StopWeapon();
        }
    }

    public void OnBowShoot(InputAction.CallbackContext context)
    {
        if (!PlayerInventory.WeaponAvailable(m_bowConfig.WeaponType)) return;
        if (TriggerInteraction()) return;

        if (context.started)
            UseWeapon(m_bowConfig);
    }

    public void OnPickSwing(InputAction.CallbackContext context)
    {
        if (!PlayerInventory.WeaponAvailable(m_pickConfig.WeaponType)) return;
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

        Reflex.Injectors.GameObjectInjector.InjectObject(gameObject, Reflex.Core.Container.RootContainer);
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

        if (CinemachineBrain.GetActiveBrain(0).ActiveBlend != null)
        {
            PlayAnimation("Idle");
            SetVelocity(Vector2.zero, false);
            var currentColor = m_renderer.color;
            currentColor.a = m_transitionBlending.Evaluate(CinemachineBrain.GetActiveBrain(0).ActiveBlend.BlendWeight);
            m_renderer.color = currentColor;

            return;
        }

        if (!IsAlive)
        {
            SetVelocity(Vector2.zero, false);
            return;
        }

        if (IsHurting)
            return;

        if(IsFalling)
        {
            if (IsAnimComplete())
                OnFallComplete?.Invoke();

            return;
        }

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

        if (CurrentVelocity == Vector2.zero)
            PlayAnimation("Idle");
        else
            PlayAnimation("Run");
    }
    #endregion

    #region Character Overrides

    public override void HealCharacter(int heal)
    {
        OnHealthChanged?.Invoke(m_maxHealth, m_currentHealth, m_currentHealth + heal);

        base.HealCharacter(heal);
    }

    public override void TakeDamage(IDamager source, int damage)
    {
        base.TakeDamage(source, damage);
        OnHealthChanged?.Invoke(m_maxHealth, Math.Min(m_currentHealth + damage, m_maxHealth), m_currentHealth);
    }

    protected override void KillCharacter(IDamager source)
    {
        base.KillCharacter(source);
        StopWeapon();
    }

    protected override void DestroyCharacter()
    {
        // TODO: Respawn the player/reload the scene instead of destroying them
        base.DestroyCharacter();
    }
    
    #endregion

    #region Weapon
    public void LaunchWeaponProjectile()
    {
        if (m_weaponMap[CurrentWeapon].Projectile != null)
        {
            Projectile projectile = PoolManager.SpawnObject(m_weaponMap[CurrentWeapon].Projectile);
            projectile.RotateToTransform(m_weaponAnimator.transform.parent);
            projectile.SetAttackData(m_weaponMap[CurrentWeapon].WeaponDamage, m_weaponMap[CurrentWeapon].WeaponKnockback);
        }
        else
        {
            Debug.LogError($"ERROR: Attempting to fire projectile from [{CurrentWeapon}/{m_weaponMap[CurrentWeapon]}] with no projectile set. ");
        }
    }

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

        if(Mathf.Abs(moveValue.x) > Mathf.Abs(moveValue.y))
            m_weaponAnimator.transform.SetParent(m_socketForwardSwing, false);
        else if (moveValue.y > 0)
            m_weaponAnimator.transform.SetParent(m_socketUpSwing, false);
        else if (moveValue.y < 0)
            m_weaponAnimator.transform.SetParent(m_socketDownSwing, false);
    }

    private bool IsInWeaponAnim(string weaponAnim)
    {
        var animHash = Animator.StringToHash(weaponAnim);
        return m_weaponAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == animHash;
    }
    #endregion
}
