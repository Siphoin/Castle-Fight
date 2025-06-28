using CastleFight.Networking.Models;

namespace CastleFight.Core
{
    public interface IOwnerable
    {
        bool IsMy {  get; }
        NetworkPlayer Owner { get; }
        ulong OwnerId { get; }
    }
}
