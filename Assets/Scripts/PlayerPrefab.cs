using Oculus.Avatar2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static Oculus.Avatar2.OvrAvatarEntity;

public class PlayerPrefab : NetworkBehaviour
{
    [SerializeField] private OvrAvatarEntity RemoteAvatarPrefab;
    private OvrAvatarEntity _remoteAvatar;
    private OvrAvatarEntity _localAvatar;
    private static readonly float[] StreamLodSnapshotIntervalSeconds = new float[OvrAvatarEntity.StreamLODCount] { 1f / 72, 2f / 72, 3f / 72, 4f / 72 };
    private readonly float[] _streamLodSnapshotElapsedTime = new float[OvrAvatarEntity.StreamLODCount];
    private const int MAX_PACKETS_PER_FRAME = 3;

    public void Start()
    {
        if (!IsLocalPlayer)
        {
            _remoteAvatar = Instantiate(RemoteAvatarPrefab, transform);
            return;
        }

        // Find the local avatar
        _localAvatar = FindObjectsOfType<OvrAvatarEntity>().FirstOrDefault(el => el.IsLocal);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendAvatarServerRpc(byte[] data, uint dataByteCount)
    {
        // Serialization of the packet data is implemented in the PacketData itself
        SetRemoteAvatarClientRpc(data, dataByteCount);
    }

    [ClientRpc]
    public void SetRemoteAvatarClientRpc(byte[] data, uint dataByteCount)
    {
        if (IsLocalPlayer)
        {
            return;
        }

        NativeArray<byte> nativeByteArray = new NativeArray<byte>(data, Allocator.Persistent);
        var dataSlice = new NativeSlice<byte>(nativeByteArray, 0, (int)dataByteCount);
        _remoteAvatar.ApplyStreamData(in dataSlice);
    }

    private void LateUpdate()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        // Local avatar has fully updated this frame and can send data to the network
        SendSnapshot();
    }

    private void SendSnapshot()
    {
        if (!_localAvatar.HasJoints) { return; }

        for (int streamLod = (int)StreamLOD.High; streamLod <= (int)StreamLOD.Low; ++streamLod)
        {
            int packetsSentThisFrame = 0;
            _streamLodSnapshotElapsedTime[streamLod] += Time.unscaledDeltaTime;
            while (_streamLodSnapshotElapsedTime[streamLod] > StreamLodSnapshotIntervalSeconds[streamLod])
            {
                SendPacket((StreamLOD)streamLod);
                _streamLodSnapshotElapsedTime[streamLod] -= StreamLodSnapshotIntervalSeconds[streamLod];
                if (++packetsSentThisFrame >= MAX_PACKETS_PER_FRAME)
                {
                    _streamLodSnapshotElapsedTime[streamLod] = 0;
                    break;
                }
            }
        }
    }

    private void SendPacket(StreamLOD lod)
    {
        var packet = GetPacketForEntityAtLOD(lod);

        packet.dataByteCount = _localAvatar.RecordStreamData_AutoBuffer(lod, ref packet.data);
        Debug.Assert(packet.dataByteCount > 0);

        if (packet.Release())
        {
            SendAvatarServerRpc(packet.data.ToArray(), packet.dataByteCount);
        }
    }

    private PacketData GetPacketForEntityAtLOD(StreamLOD lod)
    {
        PacketData packet = new PacketData();
        packet.lod = lod;
        return packet.Retain();
    }

    public class PacketData : IDisposable
    {
        public NativeArray<byte> data;
        public StreamLOD lod;
        public UInt32 dataByteCount;

        private uint refCount = 0;

        public PacketData() { }

        ~PacketData() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (data.IsCreated)
            {
                data.Dispose();
            }
            data = default;
        }

        public bool Unretained => refCount == 0;
        public PacketData Retain() { ++refCount; return this; }
        public bool Release() {
            return --refCount == 0;
        }
    }
    class LoopbackState
    {
        public List<PacketData> packetQueue = new List<PacketData>(64);
        public StreamLOD requestedLod = StreamLOD.Low;
        public float smoothedPlaybackDelay = 0f;
    };
}
