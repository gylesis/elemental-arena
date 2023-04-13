using Fusion;

namespace Dev.Infrastructure
{
    public interface IInputListener
    {
        PlayerRef InputAuthority { get; set; }
        
        void OnInput(NetworkRunner runner, NetworkInputData inputData);
    }   
}