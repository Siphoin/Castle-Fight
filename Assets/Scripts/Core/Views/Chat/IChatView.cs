namespace CastleFight.Core.Views.Chat
{
    public interface IChatView
    {
        bool IsActive { get; }
        void SetStateActive (bool active);
    }
}