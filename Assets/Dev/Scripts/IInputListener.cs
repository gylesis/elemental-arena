using Dev.Infrastructure;
using Fusion;

namespace Dev
{
    public interface IInputListener
    {
        PlayerRef InputAuthority { get; set; }
        
        void OnInput(NetworkRunner runner, NetworkInputData inputData);
    }   
}