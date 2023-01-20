using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager
{
    private ANetworkingModel _networkingModel;
    public LobbyManager(ANetworkingModel networkingModel)
    {
        _networkingModel = networkingModel;

        _networkingModel.CreateLobbyEvent += () => OnCreateLobby();
    }

    public async Task OnCreateLobby() {
        var lobbyName = GenerateRandomLobbyName();
        await CreateAndJoinLobbyAsync(lobbyName);
        _networkingModel.CurrentLobbyId = lobbyName;
    }

    private string GenerateRandomLobbyName() {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        string randomString = new string(Enumerable.Repeat(chars, 10)
          .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }

    private async Task CreateAndJoinLobbyAsync(string lobbyName) {
        int maxPlayers = 4;
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        // Heartbeat the lobby every 15 seconds.
        HeartbeatLobbyAsync(lobby.Id, 15);
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
            var queryResponses = await FetchLobbiesAsync();
            _networkingModel.AvailableLobbies = queryResponses.Results;
            await Task.Delay(waitTimeSeconds * 1000);
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
