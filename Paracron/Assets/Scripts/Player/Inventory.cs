using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryUIPresenter inventoryUIPresenter;
    [SerializeField] AudioClip increaseReserveAmmoSE;

    HashSet<string> items = new();
    Dictionary<int, int> magazineAmmo = new();
    int reserveAmmo = 0;


    OwnedWeapon ownedWeapon;
    EquippedWeapon equippedWeapon;
    AudioSource audioSource;

    public int ReserveAmmo => reserveAmmo;
    public int MagazineAmmo(int weaponInventoryIndex) => magazineAmmo.ContainsKey(weaponInventoryIndex) ? magazineAmmo[weaponInventoryIndex] : 0;


    void Awake()
    {
        ownedWeapon = GetComponent<OwnedWeapon>();
        equippedWeapon = GetComponent<EquippedWeapon>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
    }

    public void AddItem(WeaponSO weaponSO)
    {
        items.Add(weaponSO.name);
        inventoryUIPresenter.OnGetWeapon(weaponSO);
    }

    public void RemoveItem(WeaponSO weaponSO)
    {
        if (HasItem(weaponSO))
            items.Remove(weaponSO.name);
        else
            Debug.Log(weaponSO.name + "はInventoryにありませんえん二千円");
    }

    public bool HasItem(WeaponSO weaponSO)
    {
        return items.Contains(weaponSO.name);
    }

    public void AdjustMagazineAmmo(int weaponInventoryIndex, int amount)
    {
        if (!magazineAmmo.ContainsKey(weaponInventoryIndex))
        {
            magazineAmmo[weaponInventoryIndex] = 0; // 最初の最初だけ初期化
        }
        magazineAmmo[weaponInventoryIndex] = Mathf.Min(magazineAmmo[weaponInventoryIndex] + amount, equippedWeapon.CurrentWeaponSO.MagazineSize);
        inventoryUIPresenter.OnAdjustMagazineAmmo(weaponInventoryIndex);
    }

    public void AdjustReserveAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, 999);
        inventoryUIPresenter.OnAdjustReserveAmmo();
        if (amount > 0)
        {
            audioSource.PlayOneShot(increaseReserveAmmoSE);
        }
    }

    public void ReloadAmmo(WeaponSO curWeaponSO)
    {
        int amount = curWeaponSO.MagazineSize - magazineAmmo[curWeaponSO.inventoryIndex];
        amount = Mathf.Min(amount, reserveAmmo);
        AdjustMagazineAmmo(curWeaponSO.inventoryIndex, amount);
        AdjustReserveAmmo(-amount);
    }
}
