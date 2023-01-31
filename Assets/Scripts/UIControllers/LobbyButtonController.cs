using Oculus.Interaction;
using System;
using UnityEngine;

public class LobbyButtonController : ALobbyButtonController
{
    [SerializeField] private PointerInteractable<PokeInteractor, PokeInteractable> Button;

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
