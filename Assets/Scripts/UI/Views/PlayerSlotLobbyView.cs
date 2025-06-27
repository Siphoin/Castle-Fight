using CastleFight.Networking.Models;
using CastleFight.UI.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CastleFight.UI.Views
{
    public class PlayerSlotLobbyView : MonoBehaviour, IPlayerSlotLobbyView
    {
        [SerializeField] private Image _colorImage;
        [SerializeField] private TextMeshProUGUI _textNickName;
        [Inject] private TeamColorPaletteConfig _paletteConfig;

        public ulong PlayerId {  get; private set; }

        public void Hide()
        {
            PlayerId = 0;
            transform.SetParent(null);
            gameObject.SetActive(false);
            
        }

        public void SetPlayer(NetworkPlayer player)
        {
            _colorImage.color = _paletteConfig.GetColor(player.ColorType);
            _textNickName.text = player.NickName.ToString();
            PlayerId = player.ClientId;
        }
    }
}