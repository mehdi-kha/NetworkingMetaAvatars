using UnityEngine;
using UnityEngine.UI;

public class EditorNetworkMenuController : ANetworkMenuController
{
    [SerializeField] private Button _createLobbyButton;

    protected override void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
        base.Awake();
        _createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _createLobbyButton.onClick.RemoveAllListeners();
    }
}
