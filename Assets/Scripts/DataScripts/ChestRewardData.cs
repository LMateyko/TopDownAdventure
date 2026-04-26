using UnityEngine;

[CreateAssetMenu(fileName = "ChestRewardData", menuName = "Adventure Game Data/ChestRewardData")]
public class ChestRewardData : ScriptableObject
{
    public Sprite RevealSprite;
    public string RevealText;

    public PickupObject.PickupType RewardType;
    public int RewardValue;

    // TODO: Reward Audio
    // TODO: Allow for unlocking equipment etc. mine pick
}
