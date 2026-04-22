using UnityEngine;
using UnityEngine.InputSystem;

public class GameDialogRelay : MonoBehaviour
{
    [SerializeField] string[] m_dialogSequence;

    private PlayerController m_trackedPlayer;
    private DialogUI m_dialog;
    private int m_currentDialog = 0;

    public void TriggerDialogSequence()
    {
        // TODO: Have dialog or GUI manager dictate if the player can or can't have input at any given moment. 
        m_trackedPlayer.DisableInputForExternalInteraction();
        m_dialog.SetDialogText(m_dialogSequence[0]);

        m_currentDialog = 0;

        var submitAction = InputSystem.actions.FindAction("UI/Submit");
        submitAction.started += SubmitAction_started;
    }

    private void Start()
    {
        // TODO: Retrieve these values via global access when needed instead of finding them on Start
        m_trackedPlayer = FindFirstObjectByType<PlayerController>();
        m_dialog = FindFirstObjectByType<DialogUI>();
    }

    private void SubmitAction_started(InputAction.CallbackContext obj)
    {
        m_currentDialog++;
        if (m_currentDialog >= m_dialogSequence.Length)
        {
            m_trackedPlayer.ReEnableInput();
            var submitAction = InputSystem.actions.FindAction("UI/Submit");
            submitAction.started -= SubmitAction_started;
            m_dialog.SetDialogText("");
            return;
        }

        m_dialog.SetDialogText(m_dialogSequence[m_currentDialog]);
    }

}
