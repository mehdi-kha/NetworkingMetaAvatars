using UnityEngine;
using UnityEngine.UI;

public class EditorNetworkMenuController : ANetworkMenuController
{
    [SerializeField] private Button _createLobbyButton;
    [SerializeField] private Button _updateLobbiesButton;

    protected override void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        base.Awake();
        _createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        _updateLobbiesButton.onClick.AddListener(OnUpdateLobbiesButtonClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _createLobbyButton.onClick.RemoveAllListeners();
        _updateLobbiesButton.onClick.RemoveAllListeners();
    }
}
