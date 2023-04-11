using System;
using System.Collections.Generic;
using Dev.UI;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev
{
    public class ElementsView : MonoBehaviour
    {
        [SerializeField] private DefaultTextReactiveButton[] _buttons;
        
        private NetworkRunner _runner;
        private Player _player;

        private void Awake()
        {
            Array values = Enum.GetValues(typeof(ElementType));
            List<ElementType> elementTypes = new List<ElementType>(values.Length);

            for (int i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i);
                var elementType = (ElementType)value;
                
                elementTypes.Add(elementType);
            }
            
            for (var index = 0; index < _buttons.Length; index++)
            {
                DefaultTextReactiveButton button = _buttons[index];
                ElementType elementType = elementTypes[index];

                button.SetText($"Element {elementType}");
                button.Clicked.Subscribe((unit => OnButtonClick(elementType)));
            }
        }

        [Inject]
        private void Init(NetworkRunner runner)
        {
            _runner = runner;
        }
        
        private void OnButtonClick(ElementType elementType)
        {
            if (_player == null)
            {
                NetworkObject networkObject = _runner.GetPlayerObject(_runner.LocalPlayer);
                _player = networkObject.GetComponent<Player>();
            }
            
            _player.ElementsState.RPC_SetElementTypeValue(elementType);
        }
        
    }
}