using CastleFight.Core.Handlers;
using UniRx;
namespace CastleFight.Core.UI
{
    public class TimeMatchView : UIText
    {
        private IMatchHandler _matchHandler;

        private void Start()
        {
            _matchHandler = FindAnyObjectByType<MatchHandler>();
            if (_matchHandler is null)
            {
                return;
            }

            _matchHandler.OnTickMatchTime.Subscribe(time =>
            {
                Component.text = time.Hour == 0 ? time.ToString("mm:ss") : time.ToString("h:mm:ss");

            }).AddTo(this);
        }
    }
}