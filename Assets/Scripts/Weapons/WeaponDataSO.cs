using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon_New", menuName = "Game/Weapons/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Identity")]
    public string displayName = "New Weapon";

    [Header("Fire")]
    public bool automatic = false;
    public float damage = 10f;
    public float fireRate = 4f;
    public float fireDistance = 40f;

    [Header("Ammo")]
    public int magazineSize = 12;
    public int startingReserveAmmo = 48;
    public float reloadTime = 1.2f;

    [Header("Audio")]
    public AudioClip fireClip;
    public AudioClip reloadClip;
}