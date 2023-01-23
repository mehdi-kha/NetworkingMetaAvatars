using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshPro _buttonText;
    [SerializeField] private PointerInteractable<PokeInteractor, PokeInteractable> Button;

    public string Text
    {
        get => _buttonText.text;
        set => _buttonText.text = value;
    }

    public Lobby Lobby;
    public ANetworkingModel NetworkingModel;

    public event Action<Lobby> ButtonClickedEvent;

    private void Awake()
    {
        Button.WhenPointerEventRaised += OnPointerEventRaised;
    }

    private void OnDestroy()
    {
        Button.WhenPointerEventRaised -= OnPointerEventRaised;
    }

    private void OnPointerEventRaised(PointerEvent obj)
    {
        if (obj.Type != PointerEventType.Select)
        {
            return;
        }

        NetworkingModel.ConnectToLobby(Lobby);
    }
}
