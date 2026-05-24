using Reflex.Core;
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
        Player.OnFallComplete += RespawnPlayer;
    }

    private void RespawnPlayer()
    {
        Player.transform.position = RoomSpawnPosition;
        Player.ReEnableInput();
        Player.PlayAnimation("Idle");
    }
}
