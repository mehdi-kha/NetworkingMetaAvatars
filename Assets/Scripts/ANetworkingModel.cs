using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ANetworkingModel : ScriptableObject
{
    private Lobby _currentLobbyId;
    private List<Lobby> _availableLobbies = new();
    public event Action OnLocalPlayerConnected;
    public event Action OnRemotePlayerConnected;
    public event Action OnLocalPlayerDisconnected;
    public event Action OnRemotePlayerDisconnected;
    public event Action CreateLobbyEvent;
    public event Action<Lobby> LobbyChanged;
    public event Action<Lobby> ConnectToLobbyRequest;

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

    public void ConnectLocalPlayer() {
        IsLocalPlayerConnected = true;
        OnLocalPlayerConnected?.Invoke();
    }

    public void ConnectRemotePlayer(string playerId) {
        RemotePlayerIds.Add(playerId);
        OnRemotePlayerConnected?.Invoke();
    }

    public void DisconnectLocalPlayer() {
        IsLocalPlayerConnected = false;
        OnLocalPlayerDisconnected?.Invoke();
    }

    public void DisconnectRemotePlayer(string playerId) {
        RemotePlayerIds.Remove(playerId);
        OnRemotePlayerDisconnected?.Invoke();
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
