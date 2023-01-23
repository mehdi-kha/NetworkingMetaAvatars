using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    private AuthenticationManager _authenticationManager = new();
    private LobbyManager _lobbyManager;
    public ANetworkingModel NetworkingModel;

    private async void Start()
    {
        await InitializeServicesAsync();
        await _authenticationManager.Setup();
        _lobbyManager = new LobbyManager(NetworkingModel);

        // TODO improve it by passing it a cancellation token
        _lobbyManager.RefreshLobbyDataAsync(1);

        NetworkingModel.ConnectToLobbyRequest += (a) => OnConnectToLobbyRequest(a);
    }

    private async Task OnConnectToLobbyRequest(Lobby lobby)
    {
        // Connect to the lobby
        try
        {
            await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

            // Update the model, if the connection was successful
            NetworkingModel.CurrentLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task InitializeServicesAsync()
    {
        await UnityServices.InitializeAsync();
    }
}
