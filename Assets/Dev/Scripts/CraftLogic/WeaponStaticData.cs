using System;
using Dev.Weapons.Guns;
using UnityEngine;

namespace Dev.CraftLogic
{
    [Serializable]
    public class WeaponStaticData
    {
        public string Name = "Weapon";
        
        [SerializeField] private Recipe _recipe;
        [SerializeField] private Weapon _weaponPrefab;

        public Weapon WeaponPrefab => _weaponPrefab;

        public bool AreRecipesMatch(Recipe targetRecipe)
        {
            return targetRecipe.Equals(_recipe);
        }
    }
}