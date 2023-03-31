using Fusion;
using UniRx;
using UnityEngine;

namespace Dev.Weapons.Ammo
{
    public abstract class Bomb : NetworkBehaviour
    {
        [SerializeField] protected NetworkRigidbody2D _rigidbody;
        [SerializeField] protected LayerMask _playerLayerMask;
        [SerializeField] protected float _explosionRadius = 2;
        
        protected Vector2 _forcePower;
        protected float _deathTime;
        protected float _damage;
        protected float _explosionModifier;

        public Subject<Bomb> ToDestroy { get; set; } = new Subject<Bomb>();

        public virtual void Init(float deathTime, float damage, Vector2 throwDirection,
            float explosionModifier)
        {
            _explosionModifier = explosionModifier;
            _damage = damage;
            _deathTime = deathTime;
            _forcePower = throwDirection;
        }
    }
}