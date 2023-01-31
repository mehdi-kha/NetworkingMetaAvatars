using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorLobbyButtonController : ALobbyButtonController
{
    [SerializeField] private Button _button;
    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnClick()
    {
        NetworkingModel.ConnectToLobby(Lobby);
    }
}
