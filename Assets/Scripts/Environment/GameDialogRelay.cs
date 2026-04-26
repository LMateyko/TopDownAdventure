using Reflex.Attributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameDialogRelay : MonoBehaviour
{
    [SerializeField] string[] m_dialogSequence;

    [Inject] readonly private PlayerManager PlayerManager;

    public Action OnDialogComplete;

    //private PlayerController m_trackedPlayer;
    private DialogUI m_dialog;
    private int m_currentDialog = 0;

    public void TriggerDialogSequence()
    {
        TriggerDialogSequence(m_dialogSequence[0]);
    }

    public void TriggerDialogSequence(string dialog)
    {
        // TODO: Have dialog or GUI manager dictate if the player can or can't have input at any given moment. 
        PlayerManager.Player.DisableInputForExternalInteraction();
        m_dialog.SetDialogText(dialog);

        m_currentDialog = 0;

        var submitAction = InputSystem.actions.FindAction("UI/Submit");
        submitAction.started += SubmitAction_started;
    }

    private void Start()
    {
        // TODO: Retrieve these values via global access when needed instead of finding them on Start
        m_dialog = FindFirstObjectByType<DialogUI>();
    }

    private void SubmitAction_started(InputAction.CallbackContext obj)
    {
        m_currentDialog++;
        if (m_currentDialog >= m_dialogSequence.Length)
        {
            PlayerManager.Player.ReEnableInput();
            var submitAction = InputSystem.actions.FindAction("UI/Submit");
            submitAction.started -= SubmitAction_started;
            m_dialog.SetDialogText("");

            OnDialogComplete?.Invoke();

            return;
        }

        m_dialog.SetDialogText(m_dialogSequence[m_currentDialog]);
    }

}
