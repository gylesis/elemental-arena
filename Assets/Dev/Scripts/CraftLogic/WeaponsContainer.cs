using UnityEngine;

namespace Dev
{
    [CreateAssetMenu(menuName = "StaticData/WeaponsContainer", fileName = "WeaponsContainer", order = 0)]
    public class WeaponsContainer : ScriptableObject
    {
        [SerializeField] private WeaponStaticData[] _weaponDatas;

        public bool TryGetWeaponByRecipe(Recipe recipe, out WeaponStaticData weaponStaticData)
        {
            weaponStaticData = null;
            
            foreach (WeaponStaticData weaponData in _weaponDatas)
            {
                var areRecipesMatch = weaponData.AreRecipesMatch(recipe);
                
                if (areRecipesMatch)
                {
                    weaponStaticData = weaponData;
                    return true;
                }
            }

            return false;
        }
        
    }
}