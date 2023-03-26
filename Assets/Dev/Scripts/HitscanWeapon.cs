using UnityEngine;

namespace Dev
{
    public abstract class HitscanWeapon : Weapon
    {
        [SerializeField] protected float _lenght;
        [SerializeField] protected LayerMask _layerMask;
    }
}