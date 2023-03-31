using System.Collections.Generic;
using DG.Tweening;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dev.Weapons.Ammo
{
    public class ExplosiveBomb : Bomb
    {
        private TickTimer _expandTimer;
        private TickTimer _deathTimer;

        [SerializeField] private Image _deathTimerView;
        [SerializeField] private Collider2D _collider;

        private TickTimer _explosionTimer;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                _rigidbody.Rigidbody.AddForce(_forcePower, ForceMode2D.Impulse);
                RPC_ExpandView();
            }
        }   

        [Rpc]
        private void RPC_ExpandView()
        {
            _deathTimerView.fillAmount = 0;
            _deathTimerView.enabled = true;

            RPC_StartDeathTimeCountDown(_deathTime);
        }

        [Rpc]
        public void RPC_StartDeathTimeCountDown(float time)
        {
            if (Object.HasStateAuthority)
            {
                _explosionTimer = TickTimer.CreateFromSeconds(Runner, time);
            }

            DOVirtual.Float(1, 0.01f, time, (value =>
            {
                _deathTimerView.fillAmount = value;
            })).OnComplete((() =>
            {
                if (Object.HasStateAuthority)
                {
                    RPC_StartExplode();
                }
            }));
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
        private void RPC_StartExplode()
        {
            var hits = new List<LagCompensatedHit>();

            Runner.LagCompensation.OverlapSphere(transform.position, _explosionRadius, Object.InputAuthority,
                hits, _playerLayerMask);

            bool hasTarget = false;

            if (hits.Count > 0)
            {
                foreach (LagCompensatedHit hit in hits)
                {
                    if (hasTarget) break;

                    //Debug.Log($"Hit {hit.GameObject.name}");

                    var player = hit.GameObject.GetComponent<Player>();

                    PlayerRef owner = Object.InputAuthority;
                    PlayerRef target = player.Object.InputAuthority;

                    if (target == owner) continue;

                    hasTarget = true;

                    player.Damaged?.Invoke(owner, target);

                    Explode(player);
                }
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (_explosionTimer.Expired(Runner))
                {
                    ToDestroy.OnNext(this);
                }
            }
        }

        private void Explode(Player player)
        {
            var forceDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));
            forceDirection.Normalize();

            Debug.DrawRay(player.transform.position, forceDirection * 2, Color.blue, 5f);

            player.Rigidbody.Rigidbody.AddForce(forceDirection * _explosionModifier, ForceMode2D.Impulse);
        }
    }
}