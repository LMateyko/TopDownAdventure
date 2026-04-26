using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestRewardData m_chestReward;

    [Header("Component References")]
    [SerializeField] private SpriteRenderer m_revealSprite;
    [SerializeField] private Animator m_animator;
    [SerializeField] private GameDialogRelay m_dialogRelay;

    private bool m_rewarded = false;

    public void ClaimReward(Interactable interactable, PlayerController player)
    {
        if (m_rewarded) return;
        m_rewarded = true;

        m_revealSprite.sprite = m_chestReward.RevealSprite;
        m_animator.Play("Chest_Opening");

        switch (m_chestReward.RewardType)
        {
            case PickupObject.PickupType.Heart:
                player.HealCharacter(m_chestReward.RewardValue);
                break;
            case PickupObject.PickupType.Key:
                player.Keys += m_chestReward.RewardValue;
                break;
            case PickupObject.PickupType.Coin:
                player.Coins += m_chestReward.RewardValue;
                break;
        }

        m_dialogRelay.TriggerDialogSequence(m_chestReward.RevealText);
        m_dialogRelay.OnDialogComplete += HideRevealSprite;
    }

    private void Awake()
    {
        // TODO: Determine if the player has already collected this chest
    }

    private void OnValidate()
    {
        if (m_chestReward != null)
            m_revealSprite.sprite = m_chestReward.RevealSprite;
    }

    private void HideRevealSprite()
    {
        m_revealSprite.sprite = null;
        m_dialogRelay.OnDialogComplete -= HideRevealSprite;
    }

}
