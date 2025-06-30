using CastleFight.Networking.Models;

namespace CastleFight.Core
{
    public interface ITeamableObject
    {
        NetworkPlayer Owner { get; }
        bool IsAlly(ITeamableObject other);
        bool IsEnemy(ITeamableObject other);
        bool IsAlly(IOwnerable other);
        bool IsEnemy(IOwnerable other);
        bool IsAlly(NetworkPlayer player);
        bool IsEnemy(NetworkPlayer player);
    }
}
