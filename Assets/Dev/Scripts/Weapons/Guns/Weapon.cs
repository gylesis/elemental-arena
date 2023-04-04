using Fusion;
using UnityEngine;

namespace Dev.Weapons.Guns
{
    public abstract class Weapon : NetworkBehaviour
    {
        [SerializeField] protected float _cooldown = 1f;
        [SerializeField] protected float _damage = 10;
        [SerializeField] private float _shootDelay = 0;

        [SerializeField] private Transform _view;
        [Networked] public TickTimer CooldownTimer { get; set; }
        [Networked] public TickTimer ShootDelayTimer { get; set; }

        public float Cooldown => _cooldown;
        public float Damage => _damage;
        public float ShootDelay => _shootDelay;

        public virtual void StartShoot() { }
        public abstract void Shoot(Vector3 origin, Vector3 direction, float power = 1);

        public virtual void OnChosen() { }

        public virtual void SetViewState(bool isActive)
        {
            _view.gameObject.SetActive(isActive);
        }
    }
}