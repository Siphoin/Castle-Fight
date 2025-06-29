using System;
using SouthPointe.Serialization.MessagePack;
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
        public ushort Team;

        private static readonly MessagePackFormatter _formatter = new();

        internal NetworkPlayer(ulong clientId, FixedString32Bytes nickName)
        {
            ClientId = clientId;
            NickName = nickName;
            Gold = 0;
            ColorType = (int)clientId;
            IsReady = false;
            Team = 0;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                byte[] bytes = default;
                serializer.SerializeValue(ref bytes);

                if (bytes != null && bytes.Length > 0)
                {
                    NetworkPlayer deserialized = _formatter.Deserialize<NetworkPlayer>(bytes);
                    this = deserialized;
                }
            }
            else
            {
                byte[] bytes = _formatter.Serialize(this);
                serializer.SerializeValue(ref bytes);
            }
        }

        public bool Equals(NetworkPlayer other)
        {
            return ClientId == other.ClientId && NickName.Equals(other.NickName);
        }

    }
}