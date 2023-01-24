using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerPrefab : NetworkBehaviour
{
    public void Start()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        InvokeRepeating("SendClientID", 2f, 2f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendMessageServerRpc(string message)
    {
        SendMessageClientRpc(message);
    }

    [ClientRpc]
    public void SendMessageClientRpc(string message)
    {
        Debug.Log($"Message received: {message}");
    }

    void SendClientID()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            SendMessageServerRpc(NetworkManager.Singleton.LocalClientId.ToString());
        }
    }
}
