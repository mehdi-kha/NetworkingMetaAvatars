using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ANetworkingModel : ScriptableObject
{
    public event System.Action OnLocalPlayerConnected;
    public event System.Action OnRemotePlayerConnected;
    public event System.Action OnLocalPlayerDisconnected;
    public event System.Action OnRemotePlayerDisconnected;
    public event System.Action OnLobbyCreated;
    public event System.Action OnLobbyConnected;
    public event System.Action OnLobbyListUpdated;
    public bool IsLocalPlayerConnected { get; protected set; }
    public List<string> RemotePlayerIds { get; protected set; } = new List<string>();
    public List<string> AvailableLobbies { get; protected set; } = new List<string>();
    public string CurrentLobbyId { get; protected set; }

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

    public void CreateLobby() {
        // create lobby on the server
        // ...
        // set the current lobby id
        CurrentLobbyId = "lobby_id_1";
        OnLobbyCreated?.Invoke();
    }

    public void ConnectToLobby(string lobbyId) {
        // connect to the lobby on the server
        // ...
        CurrentLobbyId = lobbyId;
        OnLobbyConnected?.Invoke();
    }

    public void UpdateAvailableLobbies(List<string> newLobbies) {
        AvailableLobbies = newLobbies;
        OnLobbyListUpdated?.Invoke();
    }
}
