using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfiguration", menuName = "Adventure Game Data/WeaponConfiguration")]
public class WeaponConfiguration : ScriptableObject
{
    public enum WeaponEnum
    {
        Sword,
        Book,
        Pick,
        Bow,
        None
    }

    [Tooltip("Weapon Classification")]
    public WeaponEnum WeaponType;
    [Tooltip("Animation played when using the weapon")]
    public string WeaponAnimation;
    [Tooltip("If false, the player will maintain their facing while the weapon is in use")]
    public bool AllowFacingChange = false;
    [Tooltip("If True, the player will exit the weapon state when the animation ends")]
    public bool AnimationTransition = true;
    [Tooltip("How the player's speed is adjusted while the weapon is in use")]
    public float SpeedMultiplier = 0.75f;

    [Space]
    public int WeaponDamage = 1;
    public float WeaponKnockback = 5f;
}
