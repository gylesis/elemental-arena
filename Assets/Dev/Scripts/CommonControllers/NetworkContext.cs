using Fusion;
using UnityEngine;

namespace Dev.CommonControllers
{
    public abstract class NetworkContext : NetworkBehaviour
    {
        [Rpc]
        public void RPC_SetPos(NetworkObject networkObject, Vector3 pos)
        {
            networkObject.transform.position = pos;
        }
        
        [Rpc]
        public void RPC_SetLocalPos(NetworkObject networkObject, Vector3 pos)
        {
            networkObject.transform.localPosition = pos;
        }

        [Rpc]
        public void RPC_SetParent(NetworkObject networkObject, NetworkObject newParent)
        {
            if (newParent == null)
            {
                networkObject.transform.parent = null;
            }
            else
            {
                networkObject.transform.parent = newParent.transform;
            }
        }
        
    }
}