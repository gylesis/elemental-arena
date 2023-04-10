using Fusion;
using UnityEngine;

namespace Dev.CommonControllers
{
    public abstract class NetworkContext : NetworkBehaviour
    {
        [Rpc]
        protected void RPC_SetPos(NetworkObject networkObject, Vector3 pos)
        {
            networkObject.transform.position = pos;
        }
        
    }
}