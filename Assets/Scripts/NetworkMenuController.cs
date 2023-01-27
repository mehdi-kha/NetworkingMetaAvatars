using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworkMenuController : ANetworkMenuController
{
    [SerializeField] private float _spaceBetweenButtons; // Unfortunately, it looks like there is not easy way of having a vertical layout, hence this workaround.
    public PointableUnityEventWrapper CreateLobbyButton;
    public PointableUnityEventWrapper UpdateLobbiesButton;

    protected override void Awake()
    {
        base.Awake();
        CreateLobbyButton.WhenSelect.AddListener(OnCreateLobbyButtonClicked);
        UpdateLobbiesButton.WhenSelect.AddListener(OnUpdateLobbiesButtonClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CreateLobbyButton.WhenSelect.RemoveAllListeners();
        UpdateLobbiesButton.WhenSelect.RemoveAllListeners();
    }

    protected override void OnLobbyListUpdated(List<Lobby> addedLobbies, List<Lobby> removedLobbies)
    {
        base.OnLobbyListUpdated(addedLobbies, removedLobbies);

        // Update the positions
        var i = 0;
        foreach (var lobby in _lobbyButtons)
        {
            lobby.Value.transform.localPosition = Vector3.down * _spaceBetweenButtons * i;
            i++;
        }
    }
}
