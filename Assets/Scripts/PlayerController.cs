using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Player.IPlayerActions
{
    [Header("Character Settings")]
    [SerializeField] private float m_characterSpeed = 3f;

    [Header("Character References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private Animator m_weaponAnimator;
    [SerializeField] private Rigidbody2D m_rigidBody;

    [Header("Sword Sockets")]
    [SerializeField] private Transform m_socketUpSwing;
    [SerializeField] private Transform m_socketForwardSwing;
    [SerializeField] private Transform m_socketDownSwing;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);

    private InputSystem_Player m_playerInputSystem;
    private InputSystem_Player.PlayerActions m_playerActions;

    private void Awake()
    {
        m_playerInputSystem = new InputSystem_Player();
        m_playerActions = m_playerInputSystem.Player;
        m_playerActions.AddCallbacks(this);
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

    public void OnMove(InputAction.CallbackContext context)
    {
        var moveValue = context.ReadValue<Vector2>();

        m_rigidBody.linearVelocity = moveValue * m_characterSpeed;
        if (m_rigidBody.linearVelocity == Vector2.zero)
            m_animator.Play("Player_Idle");
        else
            m_animator.Play("Player_Run");

        // TODO: Prevent Socket adjustment and sprite flipping while swinging
        if (moveValue.y > 0)
            m_weaponAnimator.transform.SetParent(m_socketUpSwing, false);
        else if (moveValue.y < 0)
            m_weaponAnimator.transform.SetParent(m_socketDownSwing, false);
        else if (moveValue.y == 0 && moveValue.x != 0)
            m_weaponAnimator.transform.SetParent(m_socketForwardSwing, false);

        if (moveValue.x > 0)
            transform.localScale = FaceRightScale;
        else if (moveValue.x < 0)
            transform.localScale = FaceLeftScale;
    }

    public void OnSword(InputAction.CallbackContext context)
    {
        m_weaponAnimator.Play("Weapon_Sword_Attack");
    }

    public void OnAbilityOne(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnAbilityTwo(InputAction.CallbackContext context)
    {
        m_weaponAnimator.Play("Weapon_Bow_Attack");
    }

    public void OnAbilityThree(InputAction.CallbackContext context)
    {
        m_weaponAnimator.Play("Weapon_Pick_Attack");
    }
}
