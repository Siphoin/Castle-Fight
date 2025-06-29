using Unity.Netcode.Components;

namespace CastleFight.Networking.Components
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
