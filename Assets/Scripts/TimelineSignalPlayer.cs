using Reflex.Attributes;
using UnityEngine;

public class TimelineSignalPlayer : MonoBehaviour
{
    [Inject] readonly private AudioManager AudioManager;
    [Inject] readonly private PlayerManager PlayerManager;

    public void PlayerInput_Pause()
    {
        PlayerManager.Player.DisableInputForExternalInteraction();
    }

    public void PlayerInput_Resume()
    {
        PlayerManager.Player.ReEnableInput();
    }

    public void Music_Pause()
    {
        AudioManager.PauseMusic();
    }

    public void Music_Resume()
    {
        AudioManager.ResumeMusic();
    }

}
