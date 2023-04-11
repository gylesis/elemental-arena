using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Fusion;

namespace Dev
{
    public class InputListenerDispatcher
    {
        private List<IInputListener> _listeners;

        public InputListenerDispatcher(IInputListener[] listeners)
        {
            _listeners = listeners.ToList();
        }

        public void OnInput(NetworkRunner runner, NetworkInputData inputData)
        {
            _listeners.ForEach(x => x.OnInput(runner, inputData));
        }
    }
}