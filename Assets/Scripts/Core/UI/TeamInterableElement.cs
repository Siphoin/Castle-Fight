using CastleFight.Core.Handlers;
using CastleFight.Networking.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CastleFight.Core.UI
{
    public class TeamInterableElement : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private INetworkHandler _network;
        [SerializeField] private ushort _teamId;
        private ILobbyHandler _lobbyHandler;

        private void Start()
        {
            _lobbyHandler = FindAnyObjectByType<LobbyHandler>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_lobbyHandler.IsTickingTimer)
            {
                _network.Players.SetPlayerTeam(_network.LocalClientId, _teamId);
            }
        }
    }
}