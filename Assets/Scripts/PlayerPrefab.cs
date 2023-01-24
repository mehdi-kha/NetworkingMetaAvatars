using Oculus.Avatar2;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerPrefab : NetworkBehaviour
{
    [SerializeField] private OvrAvatarEntity RemoteAvatarPrefab;
    private OvrAvatarEntity _remoteAvatar;
    public void Start()
    {
        if (!IsLocalPlayer)
        {
            _remoteAvatar = Instantiate(RemoteAvatarPrefab, transform);
        }

        //InvokeRepeating("SendClientID", 2f, 2f);
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

    //void SendClientID()
    //{
    //    if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
    //    {
    //        SendMessageServerRpc(NetworkManager.Singleton.LocalClientId.ToString());
    //    }
    //}
}
