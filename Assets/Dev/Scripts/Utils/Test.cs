using DG.Tweening;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dev.Utils
{
    public class Test : NetworkBehaviour
    {
        [SerializeField] private Transform _cube;
        private Sequence _sequence;

        [Networked] private Vector3 Scale { get; set; }
        
        /*public override void Spawned()
        {
            _sequence = DOTween.Sequence();

            _sequence
                .Append(_cube.DOScale(0, 0.5f))
                .Append(_cube.DOScale(1, 0.5f))
                .SetLoops(-1)
                .Play();
        }*/

        /*private void Start()
        {
            Thread thread = new Thread((StartThread));
            thread.Start();

            Observable.EveryUpdate()
            
            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe((l =>
            {
                thread.Abort();
            }));
        }

        private void StartThread(object obj)
        {
            while (true)
            {
                Debug.Log($"bla");
            }
        }*/

        [ContextMenu("Change Color")]
        [Rpc]
        public void RPC_Test1()
        {
            _cube.GetComponent<SpriteRenderer>().color =
                new Color(Random.value, Random.value, Random.value, 1);
        }

        [ContextMenu("Set Active")]
        [Rpc]
        public void RPC_Test2()
        {
            _cube.gameObject.SetActive(!_cube.gameObject.activeSelf);
        }
        
        [ContextMenu("Sync")]
        [Rpc]
        public void RPC_Test3()
        {
            _sequence.Kill();
            
            if (Object.HasStateAuthority == false)
            {
                _cube.localScale = Scale;
                Color color = _cube.GetComponent<SpriteRenderer>().color;
                RPC_SetColor(color);
            }

            Sequence sequence = DOTween.Sequence();
            
            sequence
                .Append(_cube.DOScale(0, 0.5f))
                .Append(_cube.DOScale(1, 0.5f))
                .SetLoops(-1)
                .Play();
            _sequence = sequence;
        }

        [Rpc]
        private void RPC_SetColor(Color color)
        {
            _cube.GetComponent<SpriteRenderer>().color = color;
        }
        

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority)
            {
                Scale = _cube.localScale;
            }
        }
    }
}