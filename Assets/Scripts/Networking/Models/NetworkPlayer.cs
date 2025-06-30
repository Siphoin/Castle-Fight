using System;
using System.Text;
using SouthPointe.Serialization.MessagePack;
using Unity.Collections;
using Unity.Netcode;
namespace CastleFight.Networking.Models
{
    [Serializable]
    public struct NetworkPlayer : INetworkSerializable, IEquatable<NetworkPlayer>
    {
        public ulong ClientId;
        public Guid Guid;
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
            Guid = System.Guid.NewGuid();
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
            return Guid == other.Guid;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"id: {ClientId}");
            sb.AppendLine($"Team: {Team}");
            sb.AppendLine($"Gold: {Gold}");
            sb.AppendLine($"NickName: {NickName}");
            return sb.ToString();
        }

    }
}