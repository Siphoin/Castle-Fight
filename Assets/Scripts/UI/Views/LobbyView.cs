﻿using CastleFight.Networking.Handlers;
using CastleFight.UI.Configs;
using CastleFight.UI.Factories;
using UnityEngine;
using Zenject;
using UniRx;
using CastleFight.Networking.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using CastleFight.Main;
namespace CastleFight.UI.Views
{
    public class LobbyView : MonoBehaviour
    {
        private List<IPlayerSlotLobbyView> _views = new();
        [Inject] private IPlayerSlotLobbyViewFactory _factory;
        [Inject] private INetworkHandler _network;
        [Inject] private PlayerSlotLobbyViewFactoryConfig _factoryConfig;
        [SerializeField] private KeyValueList<ushort, RectTransform> _containers;

        private void ShowView(NetworkPlayer player)
        {
            var targetContaner = _containers[player.Team];
            var view = _factory.Create(_factoryConfig.Prefab, Vector3.zero, Quaternion.identity, targetContaner);
            view.SetPlayer(player);
            _views.Add(view);
        }

        private void Start()
        {
            _network.Players.OnPlayerAdded.Subscribe(player =>
            {
                ShowView(player);

            }).AddTo(this);

            _network.Players.OnPlayerUpdated.Subscribe(player =>
            {
                HideView(player);
                ShowView(player);

            }).AddTo(this);

            _network.Players.OnPlayerRemoved.Subscribe(player =>
            {
                HideView(player);

            }).AddTo(this);
            foreach (var player in _network.Players)
            {
                ShowView(player);
            }
        }

        private void HideView(NetworkPlayer player)
        {
            var view = _views.FirstOrDefault(x => x.PlayerId == player.ClientId);
            view?.Hide();
        }
    }

}