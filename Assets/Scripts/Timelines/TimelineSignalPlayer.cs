using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineSignalPlayer : MonoBehaviour
{
    [Inject] readonly private PoolManager PoolManager;
    [Inject] readonly private AudioManager AudioManager;
    [Inject] readonly private PlayerManager PlayerManager;

    public void PlayerInput_Pause()
    {
        PlayerManager.PausePlayer();
    }

    public void PlayerInput_Resume()
    {
        PlayerManager.ResumePlayer();
    }

    public void Music_Pause()
    {
        AudioManager.PauseMusic();
    }

    public void Music_Resume()
    {
        AudioManager.ResumeMusic();
    }

    public void PlayVFX(PlayableGraph playGraph, VfxSignalData vfxData)
    {
        var spawnedVFX = PoolManager.SpawnObject(vfxData.vfx);
        spawnedVFX.transform.position = vfxData.targetTransform.Resolve(playGraph.GetResolver()).position;
    }

}
