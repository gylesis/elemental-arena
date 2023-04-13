using System;
using System.Collections.Generic;
using Dev.CommonControllers;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Utils;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.CraftLogic
{
    [OrderAfter(typeof(PlayerChosenElementsState))]
    public class ElementsView : NetworkContext
    {
        [SerializeField] private DefaultImageButton _defaultImageButtonPrefab;
        [SerializeField] private Transform _parent;
        
        private Player _player;

        private List<DefaultImageButton> _buttons = new List<DefaultImageButton>();

        private Dictionary<ElementType, DefaultImageButton> _elementsButtons =
            new Dictionary<ElementType, DefaultImageButton>();

        private PlayersSpawner _playersSpawner;

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

            for (int index = 0; index < elementTypes.Count; index++)
            {
                DefaultImageButton button = Instantiate(_defaultImageButtonPrefab, _parent);
                ElementType elementType = elementTypes[index];

                button.SetText($"Element {elementType}");
                button.Clicked.Subscribe((unit => OnButtonClick(elementType)));

                _buttons.Add(button);
                
                _elementsButtons.Add(elementType, button);
            }
           
        }

        [Inject]
        private void Init(PlayersSpawner playersSpawner)
        {
            Debug.Log($"Init");
            _playersSpawner = playersSpawner;
        }

        private void Start()
        {
            Debug.Log($"Start");
            _playersSpawner.Spawned.Subscribe(OnPlayerSpawned);
        }

        public override void Spawned()
        {
            Debug.Log($"Spawned");
        }   

        private void OnPlayerSpawned(Player player)
        {
            Debug.Log($"Player spawned");
            _player = player;
            
            ElementType firstElement = player.ElementsState.GetElement(1);
            ElementType secondElement = player.ElementsState.GetElement(2);    

            SetButtonsSelection(firstElement, secondElement);
        }

        private void OnButtonClick(ElementType elementType)
        {
            if (_player == null)
            {
                NetworkObject networkObject = Runner.GetPlayerObject(Runner.LocalPlayer);
                _player = networkObject.GetComponent<Player>();
            }
            
            _player.ElementsState.RPC_SetElementTypeValue(elementType);

            ElementType firstElement = _player.ElementsState.GetElement(1);
            ElementType secondElement = _player.ElementsState.GetElement(2);    

            SetButtonsSelection(firstElement, secondElement);
        }

        private void SetButtonsSelection(ElementType firstElement, ElementType secondElement)
        {
            foreach (var pair in _elementsButtons)
            {
                if (pair.Key == firstElement || pair.Key == secondElement)
                {
                    pair.Value.SetImageState(true);
                }
                else
                {
                    pair.Value.SetImageState(false);
                }
            }
        }
    }
    
}