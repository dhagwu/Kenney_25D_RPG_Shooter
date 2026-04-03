using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Slots")]
    [SerializeField] private WeaponDataSO primaryWeapon;
    [SerializeField] private WeaponDataSO secondaryWeapon;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TMP_Text weaponNameText;
    [SerializeField] private TMP_Text ammoText;

    private PlayerInputHandler inputHandler;

    private WeaponDataSO[] weaponSlots = new WeaponDataSO[2];
    private int[] ammoInMag = new int[2];
    private int[] reserveAmmo = new int[2];

    private int currentWeaponIndex;
    private float nextFireTime;
    private bool isReloading;
    private float reloadEndTime;

    private void Awake()
    {
        inputHandler = GetComponent<PlayerInputHandler>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        weaponSlots[0] = primaryWeapon;
        weaponSlots[1] = secondaryWeapon;

        InitializeSlot(0);
        InitializeSlot(1);

        EquipWeapon(0);
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        HandleWeaponSwitch();
        HandleReloadTimer();
        HandleReloadInput();
        HandleFireInput();
        RefreshHUD();
    }

    private void InitializeSlot(int index)
    {
        WeaponDataSO data = weaponSlots[index];
        if (data == null)
            return;

        ammoInMag[index] = data.magazineSize;
        reserveAmmo[index] = data.startingReserveAmmo;
    }

    private void HandleWeaponSwitch()
    {
        if (inputHandler.ConsumeSlot1Pressed())
        {
            EquipWeapon(0);
        }

        if (inputHandler.ConsumeSlot2Pressed())
        {
            EquipWeapon(1);
        }
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponSlots.Length)
            return;

        if (weaponSlots[index] == null)
            return;

        currentWeaponIndex = index;
        isReloading = false;
        RefreshHUD();
    }

    private void HandleReloadTimer()
    {
        if (!isReloading)
            return;

        if (Time.time >= reloadEndTime)
        {
            FinishReload();
        }
    }

    private void HandleReloadInput()
    {
        if (!inputHandler.ConsumeReloadPressed())
            return;

        if (isReloading)
            return;

        if (!CanReloadCurrentWeapon())
            return;

        BeginReload();
    }

    private void HandleFireInput()
    {
        WeaponDataSO current = GetCurrentWeapon();
        if (current == null)
            return;

        if (isReloading)
            return;

        bool wantsToFire = current.automatic
            ? inputHandler.FireHeld
            : inputHandler.ConsumeFirePressed();

        if (!wantsToFire)
            return;

        if (Time.time < nextFireTime)
            return;

        if (ammoInMag[currentWeaponIndex] <= 0)
        {
            if (CanReloadCurrentWeapon())
            {
                BeginReload();
            }
            return;
        }

        FireCurrentWeapon(current);
    }

    private void FireCurrentWeapon(WeaponDataSO current)
    {
        ammoInMag[currentWeaponIndex]--;
        nextFireTime = Time.time + 1f / current.fireRate;

        if (AudioManager.Instance != null && current.fireClip != null)
        {
            AudioManager.Instance.PlaySFX(current.fireClip, 1f);
        }

        if (firePoint == null || mainCamera == null)
            return;

        Vector3 aimPoint = GetAimPointOnGroundPlane();
        Vector3 shotDirection = (aimPoint - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, shotDirection, out RaycastHit hit, current.fireDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(current.damage);
            }

            Debug.DrawLine(firePoint.position, hit.point, Color.red, 0.15f);
        }
        else
        {
            Debug.DrawRay(firePoint.position, shotDirection * current.fireDistance, Color.white, 0.15f);
        }
    }

    private void BeginReload()
    {
        WeaponDataSO current = GetCurrentWeapon();
        if (current == null)
            return;

        isReloading = true;
        reloadEndTime = Time.time + current.reloadTime;

        if (AudioManager.Instance != null && current.reloadClip != null)
        {
            AudioManager.Instance.PlaySFX(current.reloadClip, 1f);
        }
    }

    private void FinishReload()
    {
        isReloading = false;

        WeaponDataSO current = GetCurrentWeapon();
        if (current == null)
            return;

        int missingAmmo = current.magazineSize - ammoInMag[currentWeaponIndex];
        int ammoToLoad = Mathf.Min(missingAmmo, reserveAmmo[currentWeaponIndex]);

        ammoInMag[currentWeaponIndex] += ammoToLoad;
        reserveAmmo[currentWeaponIndex] -= ammoToLoad;
    }

    private bool CanReloadCurrentWeapon()
    {
        WeaponDataSO current = GetCurrentWeapon();
        if (current == null)
            return false;

        return ammoInMag[currentWeaponIndex] < current.magazineSize
            && reserveAmmo[currentWeaponIndex] > 0;
    }

    private WeaponDataSO GetCurrentWeapon()
    {
        if (currentWeaponIndex < 0 || currentWeaponIndex >= weaponSlots.Length)
            return null;

        return weaponSlots[currentWeaponIndex];
    }

    private void RefreshHUD()
    {
        WeaponDataSO current = GetCurrentWeapon();
        if (current == null)
            return;

        if (weaponNameText != null)
        {
            weaponNameText.text = current.displayName;
        }

        if (ammoText != null)
        {
            string reloadSuffix = isReloading ? "  (Reloading)" : "";
            ammoText.text = $"{ammoInMag[currentWeaponIndex]} / {reserveAmmo[currentWeaponIndex]}{reloadSuffix}";
        }
    }

    private Vector3 GetAimPointOnGroundPlane()
    {
        Ray ray = mainCamera.ScreenPointToRay(inputHandler.LookScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return firePoint.position + transform.forward * 30f;
    }
}