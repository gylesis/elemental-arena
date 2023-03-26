using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fusion;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dev
{
    public class StickyBomb : Bomb
    {
        [SerializeField] private Image _deathTimerView;
        [SerializeField] private Collider2D _collider;    
        
        private Transform Target { get; set; }

        [Networked] private PlayerRef TargetPlayerRef { get; set; }

        private Vector3 _stickOffset;

        private TickTimer _explosionTimer;
        private float _expandTime;

        public void Init(float expandTime, float deathTime, float damage, Vector2 throwDirection, float explosionModifier)
        {
            _expandTime = expandTime;
            base.Init(deathTime, damage, throwDirection, explosionModifier);
        }

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                _rigidbody.Rigidbody.AddForce(_forcePower, ForceMode2D.Impulse);
                RPC_ExpandView(_expandTime);
            }
        }

        [Rpc]
        private void RPC_ExpandView(float expandTime)
        {
            var originSize = _deathTimerView.rectTransform.sizeDelta.x;

            _deathTimerView.rectTransform.sizeDelta = Vector2.zero;

            _deathTimerView.fillAmount = 1;
            _deathTimerView.enabled = true;

            DOVirtual.Float(0, 1, expandTime, (value =>
                {
                    var size = Mathf.Lerp(0, originSize, value);

                    _deathTimerView.rectTransform.sizeDelta = new Vector2(size, size);
                }))
                .OnComplete((() =>
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

                    TargetPlayerRef = player.Object.InputAuthority;
                    hasTarget = true;

                    player.Damaged?.Invoke(owner, target);

                    Vector3 direction = (hit.Point - transform.position);
                    var offset = direction.magnitude;
                    Vector3 directionNormalized = direction.normalized;

                    RPC_SetTargetToFollow(target.PlayerId, offset, directionNormalized);
                }
            }

            RPC_StartDeathTimeCountDown(_deathTime);
        }

        [Rpc]
        public void RPC_StartDeathTimeCountDown(float time)
        {
            if (Object.HasStateAuthority)
            {
                _explosionTimer = TickTimer.CreateFromSeconds(Runner, time);
            }
            
            DOVirtual.Float(1, 0.01f, time, (value => { _deathTimerView.fillAmount = value; }));
        }

        [Rpc]
        private void RPC_SetTargetToFollow(int id, float offset, Vector3 direction)
        {
            var players = FindObjectsOfType<Player>();

            Player targetPlayer = players.First(x => x.Object.InputAuthority.PlayerId == id);

            Target = targetPlayer.transform;
            _stickOffset = direction * offset;

            _collider.enabled = false;
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                if (_explosionTimer.Expired(Runner))
                {
                    if (Target)
                    {
                        NetworkObject networkObject = Runner.GetPlayerObject(TargetPlayerRef);
                        Player player = networkObject.GetComponent<Player>();
                        Explode(player);
                    }
                    ToDestroy.OnNext(this);
                }
            }

            if (Target == null) return;

            transform.position = (Target.position + transform.right + _stickOffset);
        }

        private void Explode(Player player)
        {
            var forceDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 1f));
            forceDirection.Normalize();

            Debug.DrawRay(player.transform.position, forceDirection * 2, Color.blue, 5f);

            player.Rigidbody.Rigidbody.AddForce(forceDirection * _explosionModifier, ForceMode2D.Impulse);
        }

        [Rpc]
        public void RPC_Detach()
        {
            Target = null;
        }
    }
}