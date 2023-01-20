using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyButtonController : MonoBehaviour
{
    [SerializeField] private TextMeshPro _buttonText;

    public string Text
    {
        get => _buttonText.text;
        set => _buttonText.text = value;
    }

    public event Action ButtonClickedEvent;
}
