using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private ANetworkingModel _networkingModel;
    private void Start()
    {
        _networkingModel.CreateLobbyEvent += async () => await OnCreateLobby();
        _networkingModel.RefreshLobbiesRequested += async () => await RefreshLobbies();
        _networkingModel.ConnectToLobbyRequest += async (lobby) => await OnConnectToLobbyRequest(lobby);
    }

    public async Task OnCreateLobby()
    {
        var lobbyName = GenerateRandomLobbyName();
        var allocationAndJoinCode = await RelayManager.AllocateRelayServerAndGetJoinCode();
        RelayManager.ConfigureTransportAndStartHost(allocationAndJoinCode.Item1);
        _networkingModel.CurrentLobby = await CreateAndJoinLobbyAsync(lobbyName, allocationAndJoinCode.Item2);
    }

    private string GenerateRandomLobbyName() {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomString = new string(Enumerable.Repeat(chars, 10)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }

    private async Task<Lobby> CreateAndJoinLobbyAsync(string lobbyName, string joinCode)
    {
        int maxPlayers = 4;
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Data = new Dictionary<string, DataObject>()
        {
            {
                "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)
            },
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        // Heartbeat the lobby every 15 seconds.
        _ = HeartbeatLobbyAsync(lobby.Id, 15);

        return lobby;
    }

    private async Task HeartbeatLobbyAsync(string lobbyId, int waitTimeSeconds)
    {
        // TODO: Implement a way of stopping it when the lobby is destroyed for example
        while (true)
        {
            await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            await Task.Delay(waitTimeSeconds * 1000);
        }
    }

    /// <summary>
    ///     Fetch the available lobbies and updates the model with the new list
    /// </summary>
    /// <param name="waitTimeSeconds">How often should this list be updated</param>
    /// <returns></returns>
    public async Task RefreshLobbyDataAsync(int waitTimeSeconds)
    {
        while (true)
        {
            await RefreshLobbies();
            await Task.Delay(waitTimeSeconds * 1000);
        }
    }

    private async Task RefreshLobbies()
    {
        var queryResponses = await FetchLobbiesAsync();
        _networkingModel.AvailableLobbies = queryResponses.Results;
    }

    public async Task OnConnectToLobbyRequest(Lobby lobby) {
        // Connect to the lobby
        try
        {
            var resultLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            var joinCode = resultLobby.Data["joinCode"].Value;
            await RelayManager.StartClientAsync(joinCode);

            // Update the model, if the connection was successful
            _networkingModel.CurrentLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task<QueryResponse> FetchLobbiesAsync()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            return await Lobbies.Instance.QueryLobbiesAsync(options);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }
}
