using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Adventure Game Data/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public Projectile Prefab;

    public int Damage = 1;
    public float Knockback = 3;
}
