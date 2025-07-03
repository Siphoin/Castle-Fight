using System;
using CastleFight.Networking.Models;

namespace CastleFight.Networking.Handlers
{
    public interface IChatHandler
    {
        void SendMessageToChat(string message);
        IObservable<ChatMessage> OnNewMessage { get; }
    }
}