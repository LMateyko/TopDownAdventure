using Reflex.Core;
using System;
using UnityEngine;

public class PlayerManager 
{
    public PlayerController Player { get; private set; } = null;
    public Vector3 RoomSpawnPosition { get; private set; }

    public void MoveToRoom(Vector3 transitionVector)
    {
        Player.transform.position += transitionVector;
        RoomSpawnPosition = Player.transform.position;
    }

    public void SpawnPlayer(PlayerController PlayerPrefab)
    {
        Player = GameObject.Instantiate(PlayerPrefab);
        Player.OnFallComplete += RespawnPlayerFromPit;
        Player.OnDestroyCharacter += RespawnPlayerFromDeath;
    }

    public void PausePlayer()
    {
        Player.DisableInputForExternalInteraction();
    }

    public void ResumePlayer()
    {
        Player.ReEnableInput();
    }

    private void RespawnPlayerFromPit()
    {
        Player.transform.position = RoomSpawnPosition;
        ResumePlayer();
        Player.PlayAnimation("Idle");
    }

    private void RespawnPlayerFromDeath(BaseCharacterController controller)
    {
        ResumePlayer();
        Player.RestoreCharacter();
    }
}
