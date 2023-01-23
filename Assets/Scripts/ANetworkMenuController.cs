using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public abstract class ANetworkMenuController : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _lobbyIdText;
    [SerializeField] protected ALobbyButtonController LobbyButtonPrefab;
    [SerializeField] protected Transform _buttonsParent;
    protected Dictionary<Lobby, ALobbyButtonController> _lobbyButtons;

    public ANetworkingModel NetworkingModel;

    protected virtual void Awake()
    {
        NetworkingModel.LobbyChanged += OnLobbyChanged;
        NetworkingModel.LobbyListUpdated += OnLobbyListUpdated;
    }

    protected virtual void OnDestroy()
    {
        NetworkingModel.LobbyChanged -= OnLobbyChanged;
        NetworkingModel.LobbyListUpdated -= OnLobbyListUpdated;
    }

    protected void OnCreateLobbyButtonClicked()
    {
        NetworkingModel.CreateLobby();
    }

    protected virtual void OnLobbyListUpdated(List<Lobby> addedLobbies, List<Lobby> removedLobbies)
    {
        foreach (var lobby in addedLobbies)
        {
            var instantiatedButton = Instantiate(LobbyButtonPrefab, _buttonsParent);
            instantiatedButton.Text = lobby.Name;
            instantiatedButton.Lobby = lobby;
            instantiatedButton.NetworkingModel = NetworkingModel;
            _lobbyButtons[lobby] = instantiatedButton;
        }

        foreach (var lobby in removedLobbies)
        {
            if (_lobbyButtons.ContainsKey(lobby))
            {
                Destroy(_lobbyButtons[lobby]);
                _lobbyButtons.Remove(lobby);
            }
        }
    }

    protected void OnLobbyChanged(Lobby lobby)
    {
        _lobbyIdText.text = lobby.Name;
    }
}
