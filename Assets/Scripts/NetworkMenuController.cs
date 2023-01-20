using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class NetworkMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyIdText;
    [SerializeField] private LobbyButtonController LobbyButtonPrefab;
    [SerializeField] private Transform _buttonsParent;
    private Dictionary<Lobby, LobbyButtonController> _lobbyButtons;
    [SerializeField] private float _spaceBetweenButtons; // Unfortunately, it looks like there is not easy way of having a vertical layout, hence this workaround.

    public ANetworkingModel NetworkingModel;
    public PointableUnityEventWrapper CreateLobbyButton;

    private void Awake()
    {
        CreateLobbyButton.WhenSelect.AddListener(OnCreateLobbyButtonClicked);
        NetworkingModel.LobbyIdChanged += OnLobbyIdChanged;
        NetworkingModel.LobbyListUpdated += OnLobbyListUpdated;
    }

    private void OnDestroy()
    {
        CreateLobbyButton.WhenSelect.RemoveAllListeners();
        NetworkingModel.LobbyIdChanged -= OnLobbyIdChanged;
        NetworkingModel.LobbyListUpdated -= OnLobbyListUpdated;
    }

    private void OnCreateLobbyButtonClicked()
    {
        NetworkingModel.CreateLobby();
    }

    private void OnLobbyListUpdated(List<Lobby> addedLobbies, List<Lobby> removedLobbies)
    {
        foreach (var lobby in addedLobbies)
        {
            var instantiatedButton = Instantiate(LobbyButtonPrefab, _buttonsParent);
            instantiatedButton.Text = lobby.Name;
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

        // Update the positions
        var i = 0;
        foreach (var lobby in _lobbyButtons)
        {
            lobby.Value.transform.localPosition = Vector3.down * _spaceBetweenButtons * i;
            i++;
        }
    }

    private void OnLobbyIdChanged(string lobbyId)
    {
        _lobbyIdText.text = lobbyId;
    }
}
