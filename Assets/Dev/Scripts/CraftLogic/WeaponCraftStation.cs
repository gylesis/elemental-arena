using Dev.Weapons.Guns;
using Fusion;
using UnityEngine;

namespace Dev
{
    public class WeaponCraftStation
    {
        private WeaponCraftRecipesContainer _weaponCraftRecipesContainer;
        private WeaponsContainer _weaponsContainer;

        public WeaponCraftStation(WeaponCraftRecipesContainer weaponCraftRecipesContainer,
            WeaponsContainer weaponsContainer)
        {
            _weaponsContainer = weaponsContainer;
            _weaponCraftRecipesContainer = weaponCraftRecipesContainer;
        }

        public void TryCraft(WeaponCraftContext craftContext)
        {
            ElementType firstElement = craftContext.FirstElement;
            ElementType secondElement = craftContext.SecondElement;
            NetworkRunner runner = craftContext.Runner;
            Player player = craftContext.Player;

            if (runner.IsServer == false) return;

            var tryGetRecipe = _weaponCraftRecipesContainer.TryGetRecipe(firstElement, secondElement, out var recipe);

            if (tryGetRecipe == false)
            {
                Debug.Log($"Recipe not found");
                return;
            }

            var tryGetWeaponByRecipe = _weaponsContainer.TryGetWeaponByRecipe(recipe, out var weaponStaticData);

            if (tryGetWeaponByRecipe == false)
            {
                Debug.Log($"Weapon not found");
                return;
            }

            Weapon weaponPrefab = weaponStaticData.WeaponPrefab;

            var hasWeapon = player.WeaponController.HasWeapon(weaponStaticData.Name);
            
            if(hasWeapon)
            {
                Debug.Log($"Weapon is already in use!");
                return;
            }

            Weapon weapon = runner.Spawn(weaponPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer, (OnBeforeSpawned ));

            void OnBeforeSpawned(NetworkRunner networkRunner, NetworkObject obj)
            {
                var weapon = obj.GetComponent<Weapon>();
                var weaponData = new WeaponData();
                
                weaponData.Id = weaponStaticData.GetHashCode();
                weaponData.Name = weaponStaticData.WeaponPrefab.name;
                
                weapon.Init(weaponData);
            }

            Debug.Log($"Crafted Weapon: {weaponStaticData.Name}");
            
            player.WeaponController.RPC_AddWeapon(weapon);
            player.WeaponController.RPC_ChooseWeapon(player.WeaponController.WeaponsAmount);

            weapon.RPC_SetParent(craftContext.WeaponSpawnParent);
            weapon.RPC_SetLocalPos(Vector3.zero);
            weapon.RPC_SetRotation(Vector3.zero);
        }
        
    }
}