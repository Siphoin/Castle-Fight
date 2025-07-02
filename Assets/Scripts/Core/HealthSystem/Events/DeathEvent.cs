using Unity.Netcode;

namespace CastleFight.Core.HealthSystem.Events
{
    public struct DeathEvent : INetworkSerializable
    {
        public ulong IdObject;

        public DeathEvent(ulong idObject)
        {
            IdObject = idObject;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IdObject);
        }
    }
}
