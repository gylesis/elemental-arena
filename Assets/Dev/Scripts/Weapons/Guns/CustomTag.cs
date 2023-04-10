using UnityEngine;

namespace Dev.Weapons.Guns
{
    public class CustomTag : MonoBehaviour
    {
        [SerializeField] private TagType _tagType;
        public TagType TagType => _tagType;
    }
}