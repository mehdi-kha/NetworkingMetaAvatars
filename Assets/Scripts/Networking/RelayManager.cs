using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;

public static class RelayManager
{
    public const int MaxConnections = 4;

    // Host
    public static async Task<(Allocation,string)> AllocateRelayServerAndGetJoinCode(string region = null)
    {
        Allocation allocation;
        string createJoinCode;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections, region);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try
        {
            createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        return (allocation, createJoinCode);
    }

    public static void ConfigureTransportAndStartHost(Allocation allocation)
    {
        Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
            allocation.RelayServer.IpV4, 
            (ushort) allocation.RelayServer.Port, 
            allocation.AllocationIdBytes, 
            allocation.Key,
            allocation.ConnectionData
        );
        Unity.Netcode.NetworkManager.Singleton.StartHost();
    }

    // Client

    public static async Task<JoinAllocation> JoinRelayServerFromJoinCode(string joinCode)
    {
        JoinAllocation allocation;
        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        return allocation;
    }

    public async static Task StartClientAsync(string joinCode)
    {
        var allocation = await JoinRelayServerFromJoinCode(joinCode);

        Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData
        );

        Unity.Netcode.NetworkManager.Singleton.StartClient();
    }
}
