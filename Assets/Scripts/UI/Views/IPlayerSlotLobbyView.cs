using CastleFight.Main.Factories;
using CastleFight.Networking.Models;

namespace CastleFight.UI.Views
{
    public interface IPlayerSlotLobbyView : IFactoryObject
    {
        public void SetPlayer(NetworkPlayer player);
        void Hide();

        public ulong PlayerId { get; }
    }
}