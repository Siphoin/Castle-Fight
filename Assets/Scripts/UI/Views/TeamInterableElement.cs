using System.Collections;
using CastleFight.Networking.Handlers;
using CastleFight.Networking.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CastleFight.UI.Views
{
    public class TeamInterableElement : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private INetworkHandler _network;
        [SerializeField] private ushort _teamId;

        public void OnPointerClick(PointerEventData eventData)
        {
            _network.Players.SetPlayerTeam(_network.LocalClientId, _teamId);
        }
    }
}