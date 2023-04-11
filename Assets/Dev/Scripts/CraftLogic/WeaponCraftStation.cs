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

        public void Craft(WeaponCraftContext craftContext)
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

            Weapon weapon = runner.Spawn(weaponPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer);

            player.WeaponController.Weapons.Add(weapon);

            weapon.RPC_SetParent(weapon.Object, craftContext.WeaponSpawnParent);
            weapon.RPC_SetLocalPos(weapon.Object, Vector3.zero);
        }
    }
}