using Dev.Weapons.ArmorLogic;
using UnityEngine;

namespace Dev.Weapons.Guns.Staffs
{
    public abstract class ArmorStaffWeapon : StaffWeapon
    {
        [SerializeField] protected Armor _armorPrefab; 
    }
}