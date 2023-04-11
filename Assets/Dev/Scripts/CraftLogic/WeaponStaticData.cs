using System;
using Dev.Weapons.Guns;
using UnityEngine;

namespace Dev
{
    [Serializable]
    public class WeaponStaticData
    {
        [SerializeField] private Recipe _recipe;
        [SerializeField] private Weapon _weaponPrefab;

        public Weapon WeaponPrefab => _weaponPrefab;

        public bool AreRecipesMatch(Recipe targetRecipe)
        {
            return targetRecipe.Equals(_recipe);
        }
        
    }
}