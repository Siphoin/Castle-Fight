using System;
using Unity.Collections;
using Unity.Netcode;
namespace CastleFight.Networking.Models
{
    [Serializable]
    public struct NetworkPlayer : INetworkSerializable, IEquatable<NetworkPlayer>
    {
        public ulong ClientId;
        public uint Gold;
        public FixedString32Bytes NickName;
        public int ColorType;
        public bool IsReady;

        internal NetworkPlayer(ulong clientId, FixedString32Bytes nickName)
        {
            ClientId = clientId;
            NickName = nickName;
            Gold = 0;
            ColorType = (int)clientId;
            IsReady = false;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref Gold);
            serializer.SerializeValue(ref NickName);
            serializer.SerializeValue(ref ColorType);
            serializer.SerializeValue(ref IsReady);
        }

        public bool Equals(NetworkPlayer other)
        {
            return ClientId == other.ClientId && NickName.Equals(other.NickName);
        }

    }
}