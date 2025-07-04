using CastleFight.Main.Factories;

namespace CastleFight.Core.Views
{
    public interface IClickableView : IFactoryObject
    {
        void SetVisibleSelect(bool visible);
    }
}