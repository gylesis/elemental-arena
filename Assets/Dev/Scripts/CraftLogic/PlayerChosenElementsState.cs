using System.Collections.Generic;
using Dev.CommonControllers;
using Fusion;
using UnityEngine;

namespace Dev
{
    public class PlayerChosenElementsState : NetworkContext
    {
        private Queue<ElementType> _elementTypesStack = new Queue<ElementType>(2);

        public override void Spawned()
        {
            RPC_SetElementTypeValue(ElementType.Electricity);
            RPC_SetElementTypeValue(ElementType.Water);
        }

        public ElementType GetElement(int index)
        {
            index = Mathf.Clamp(index - 1, 0, 1);
            
            for (var i = 0; i < _elementTypesStack.Count; i++)
            {
                if (i == index)
                {
                    return _elementTypesStack.ToArray()[i];
                }
            }

            Debug.Log($"Something went wrong");
            return ElementType.DefensiveMaterial;
        }

        [Rpc]
        public void RPC_SetElementTypeValue(ElementType elementType)
        {
            if (_elementTypesStack.Count >= 2)
            {
                _elementTypesStack.Dequeue();
            }
            
            _elementTypesStack.Enqueue(elementType);

            if(_elementTypesStack.Count < 2) return;

            if (Object.HasInputAuthority)
            {
                var elementTypes = _elementTypesStack.ToArray();
                Debug.Log($"Current elements:\n 1 - {elementTypes[0]}, 2 - {elementTypes[1]}");
            }
        }
        
    }
}