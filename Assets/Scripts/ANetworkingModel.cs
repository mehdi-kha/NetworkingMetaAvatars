using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public abstract class ANetworkingModel : ScriptableObject
{
    private Lobby _currentLobbyId;
    private List<Lobby> _availableLobbies = new();
    private bool _isAuthenticated;
    public event Action CreateLobbyEvent;
    public event Action<Lobby> LobbyChanged;
    public event Action<Lobby> ConnectToLobbyRequest;
    public event Action RefreshLobbiesRequested;
    public event Action<bool> AuthenticatedEvent;

    /// <summary>
    ///     Triggered when there are new or removed lobbies. The first argument are the added lobbies,
    ///     the second corresponds to the removed ones.
    /// </summary>
    public event Action<List<Lobby>, List<Lobby>> LobbyListUpdated;
    public bool IsLocalPlayerConnected { get; protected set; }
    public List<string> RemotePlayerIds { get; protected set; } = new List<string>();
    public List<Lobby> AvailableLobbies
    {
        get => _availableLobbies;
        set
        {
            var addedLobbies = value.Except(_availableLobbies).ToList();
            var removedLobbies = _availableLobbies.Except(value).ToList();

            _availableLobbies = value;

            if (addedLobbies.Count != 0 || removedLobbies.Count != 0)
            {
                LobbyListUpdated?.Invoke(addedLobbies, removedLobbies);
            }
        }
    }
    public Lobby CurrentLobby
    {
        get => _currentLobbyId;
        set
        {
            _currentLobbyId = value;
            LobbyChanged?.Invoke(value);
        }
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set
        {
            _isAuthenticated = value;
            AuthenticatedEvent?.Invoke(value);
        }
    }

    public void RefreshLobbies()
    {
        RefreshLobbiesRequested?.Invoke();
    }

    public void CreateLobby()
    {
        CreateLobbyEvent?.Invoke();
    }

    public void ConnectToLobby(Lobby lobby)
    {
        ConnectToLobbyRequest?.Invoke(lobby);
    }
}
