using CastleFight.Networking.Handlers;
using Zenject;
using UniRx;
using CastleFight.Extensions;
using System;
using CastleFight.UI.Configs;
using DG.Tweening;
using UnityEngine;
using CastleFight.Core.UI;

namespace CastleFight.Share.UI.Views
{
    public class GoldView : UIText
    {
        [Inject] private INetworkHandler _network;
        [Inject] private GoldViewConfig _config;
        private Guid _idPlayer;
        private uint _lastGoldValue;
        private Color _defaultTextColor;

        private void Awake()
        {
            _idPlayer = _network.Players.LocalPlayer.Guid;
            _defaultTextColor = Component.color;
            _lastGoldValue = _network.Players.LocalPlayer.Gold;

            _network.Players.OnPlayerUpdated.Subscribe(player =>
            {
                if (player.Guid == _idPlayer)
                {
                    var currentGold = player.Gold;
                    if (currentGold != _lastGoldValue)
                    {
                        AnimateGoldChange(currentGold);
                        _lastGoldValue = currentGold;
                    }
                    else
                    {
                        UpdateValue();
                    }
                }
            }).AddTo(this);
        }

        private void Start()
        {
            UpdateValue();
        }

        private void AnimateGoldChange(uint newGoldValue)
        {
            if (!_network.IsConnected) return;

            bool isIncrease = newGoldValue > _lastGoldValue;
            Color changeColor = isIncrease ? _config.ColorIncrement : _config.ColorDecrement;

            Component.DOKill();

            var sequence = DOTween.Sequence();
            sequence.Append(Component.DOColor(changeColor, _config.SpeedAnimation / 2).SetEase(_config.Ease));
            sequence.Append(Component.DOColor(_defaultTextColor, _config.SpeedAnimation / 2).SetEase(_config.Ease));
            sequence.OnComplete(() => UpdateValue());
            sequence.Play();
        }

        private void UpdateValue()
        {
            if (!_network.IsConnected)
            {
                return;
            }
            Component.text = _network.Players.LocalPlayer.Gold.ToGameCurrency();
        }
    }
}