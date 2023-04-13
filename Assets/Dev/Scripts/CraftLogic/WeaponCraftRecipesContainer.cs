using UnityEngine;

namespace Dev.CraftLogic
{
    [CreateAssetMenu(menuName = "StaticData/Crafts/WeaponCraftRecipes", fileName = "WeaponCraftRecipes", order = 0)]
    public class WeaponCraftRecipesContainer : ScriptableObject      
    {
        [SerializeField] private Recipe[] _recipes;

        public bool TryGetRecipe(ElementType firstElement, ElementType secondElement, out Recipe recipe)
        {
            recipe = null;
            
            foreach (Recipe rcp in _recipes)
            {
                var isRelevant = rcp.IsRelevant(firstElement, secondElement);

                if (isRelevant)
                {
                    recipe = rcp;
                    return true;
                }
            }

            return false;
        }
        
        
    }
}