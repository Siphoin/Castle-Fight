using CastleFight.Networking.Models;

namespace CastleFight.Networking.Models
{
    public struct ChatMessage
    {
        internal ChatMessage(NetworkPlayer sender, string message)
        {
            Sender = sender;
            Message = message;
        }

        public NetworkPlayer Sender { get; internal set; }
        public string Message { get; internal set; }


    }
}