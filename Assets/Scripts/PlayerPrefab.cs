using Oculus.Avatar2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using static Oculus.Avatar2.OvrAvatarEntity;

public class PlayerPrefab : NetworkBehaviour
{
    [SerializeField] private SampleAvatarEntity RemoteAvatarPrefab;
    private SampleAvatarEntity _remoteAvatar;
    private SampleAvatarEntity _localAvatar;
    private bool _isRemoteAvatarLoaded;

    // This value will only be filled for the local player
    private ulong _userId;
    WaitForSeconds waitTime = new WaitForSeconds(.08f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkDespawn();
        if (!IsLocalPlayer)
        {
            // Will only run on the player prefabs that are not the current local player
            _remoteAvatar = Instantiate(RemoteAvatarPrefab, transform);
            return;
        }

        // Else, if we are in the local player's prefab, do the following
        // Find the local avatar
        _localAvatar = FindObjectsOfType<SampleAvatarEntity>().FirstOrDefault(el => el.IsLocal);

        // Set the oculus id
        OvrPlatformInit.InitializeOvrPlatform();
        Oculus.Platform.Users.GetLoggedInUser().OnComplete(message =>
        {
            if (!message.IsError)
            {
                _userId = message.Data.ID;
            }
            else
            {
                var e = message.GetError();
                OvrAvatarLog.LogError($"Error loading CDN avatar: {e.Message}. Falling back to local avatar");
            }
        });

        // Start streaming
        StartCoroutine(StreamAvatarData());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendAvatarServerRpc(byte[] data, ulong userId)
    {
        // Serialization of the packet data is implemented in the PacketData itself
        SetRemoteAvatarClientRpc(data, userId);
    }

    [ClientRpc]
    public void SetRemoteAvatarClientRpc(byte[] data, ulong userId)
    {
        if (IsLocalPlayer)
        {
            return;
        }

        if (!_isRemoteAvatarLoaded)
        {
            _remoteAvatar.LoadRemoteUserCdnAvatar(userId);
            _isRemoteAvatarLoaded = true;
        }

        _remoteAvatar.ApplyStreamData(data);
    }

    private IEnumerator StreamAvatarData()
    {
        while(true)
        {
            var data = _localAvatar.RecordStreamData(_localAvatar.activeStreamLod);
            SendAvatarServerRpc(data, _userId);
            yield return waitTime;
        }
    }
}
