using UnityEngine;

namespace Dev.CraftLogic
{
    [CreateAssetMenu(menuName = "StaticData/Crafts/WeaponRecipe", fileName = "WeaponRecipe", order = 0)]
    public class Recipe : ScriptableObject  
    {
        [SerializeField] private ElementType _firstElement;
        [SerializeField] private ElementType _secondElement;

        public ElementType FirstElement => _firstElement;
        public ElementType SecondElement => _secondElement;

        public bool IsRelevant(ElementType firstElement, ElementType secondElement)
        {
            return (firstElement == _firstElement && secondElement == _secondElement) || (secondElement == _firstElement && firstElement == _secondElement);
        }
    }
}