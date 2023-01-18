using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMenuController : MonoBehaviour
{
    public ANetworkingModel NetworkingModel;
    public PointableUnityEventWrapper CreateLobbyButton;

    private void Awake()
    {
        CreateLobbyButton.WhenSelect.AddListener(OnCreateLobbyButtonClicked);
    }

    private void OnDestroy()
    {
        CreateLobbyButton.WhenSelect.RemoveAllListeners();
    }

    private void OnCreateLobbyButtonClicked()
    {
        NetworkingModel.CreateLobby();
    }
}
