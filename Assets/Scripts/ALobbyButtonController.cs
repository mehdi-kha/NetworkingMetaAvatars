using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ALobbyButtonController : MonoBehaviour
{
    [SerializeField] protected TMP_Text _buttonText;
    public string Text
    {
        get => _buttonText.text;
        set => _buttonText.text = value;
    }

    public Lobby Lobby;
    public ANetworkingModel NetworkingModel;
}
