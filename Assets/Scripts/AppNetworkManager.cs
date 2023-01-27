using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public class AppNetworkManager : MonoBehaviour
{
    [SerializeField] private AuthenticationManager _authenticationManager;
    [SerializeField] private VivoxManager _vivoxManager;
    private LobbyManager _lobbyManager;
    public ANetworkingModel NetworkingModel;

    private async void Start()
    {
        while (!_authenticationManager.IsReady)
        {
            await Task.Delay(100);
        }

        _lobbyManager = new LobbyManager(NetworkingModel);

        // TODO improve it by passing it a cancellation token
        //_lobbyManager.RefreshLobbyDataAsync(5);

        NetworkingModel.ConnectToLobbyRequest += async (lobby) =>
        { 
            await _lobbyManager.OnConnectToLobbyRequest(lobby);

        };

        // Let's just use the Unity authentication player id as the username in Vivox
        NetworkingModel.LobbyChanged += lobby => _vivoxManager.Login(AuthenticationService.Instance.PlayerId);
    }
}
