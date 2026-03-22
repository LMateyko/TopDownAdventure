using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Player.IPlayerActions
{
    [Header("Character References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private SpriteRenderer m_renderer;

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
        if (moveValue.x > 0)
            transform.localScale = FaceRightScale;
        else if (moveValue.x < 0)
            transform.localScale = FaceLeftScale;
    }

    public void OnSword(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnAbilityOne(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnAbilityThree(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnAbilityTwo(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

}
