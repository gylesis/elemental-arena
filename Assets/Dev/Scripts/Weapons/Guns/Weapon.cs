using Fusion;
using UnityEngine;

namespace Dev
{
    public abstract class Weapon : NetworkBehaviour
    {
        [SerializeField] protected float _cooldown = 1f;
        [SerializeField] protected float _damage = 10;

        [SerializeField] private Transform _view;
        
        [Networked] public TickTimer CooldownTimer { get; set; }
        
        public float Cooldown => _cooldown;
        public float Damage => _damage;

        public abstract void Shoot(Vector3 origin, Vector3 direction);

        public virtual void OnChosen() { }

        public virtual void SetViewState(bool isActive)
        {
            _view.gameObject.SetActive(isActive);
        }
        
    }
}