using CastleFight.Core.Handlers;
using UniRx;
using Zenject;
using CastleFight.Core.Configs;
namespace CastleFight.Core.UI
{
    public class TeamScoreView : UIText
    {
        private IMatchHandler _matchHandler;
        [Inject] private TeamScoreViewConfig _config;

        private void Start()
        {
            _matchHandler = FindAnyObjectByType<MatchHandler>();

            if (_matchHandler is null)
            {
                return;
            }

            _matchHandler.OnTeamsChanged.Subscribe(_ =>
            {
                UpdateScore();

            }).AddTo(this);

            UpdateScore();
        }

        private  void UpdateScore ()
        {
            var teamFirstScore = _matchHandler.ScoresTeams[0];
            var teamSecondScore = _matchHandler.ScoresTeams[1];

            var colorFirstTeam = _config.ColorFirstTeam;
            var colorSecondTeam = _config?.ColorSecondTeam;
            Component.text = $"<color={colorFirstTeam}>{teamFirstScore}</color> : <color={colorSecondTeam}>{teamSecondScore}</color>";
        }
    }
}