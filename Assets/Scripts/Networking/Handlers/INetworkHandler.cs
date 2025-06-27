namespace CastleFight.Networking.Handlers
{
    public interface INetworkHandler
    {
        void StartClient(string targetIP);
        void StartServer();
        void Disconnect();
    }
}