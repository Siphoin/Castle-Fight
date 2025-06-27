using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using CastleFight.Networking.Handlers;
namespace CastleFight.UI.Screens
{
    public class ConnectScreen : UIScreen
    {
        [SerializeField] private Button _buttonConnect;
        [SerializeField] private Button _buttonStartHost;

        [SerializeField] private UIInput _inputAddress;
        [SerializeField] private UIInput _inputName;

        [Inject] private INetworkHandler _networkHandler;

        private void Awake()
        {
            _buttonStartHost.onClick.AsObservable().Subscribe(_ =>
            {
                StartHost();

            }).AddTo(this);

            _buttonConnect.onClick.AsObservable().Subscribe(_ =>
            {
                Connect();

            }).AddTo(this);
        }

        private void Connect()
        {
            _networkHandler.StartClient(_inputAddress.Text, _inputName.Text);
        }

        private void StartHost()
        {
            _networkHandler.StartServer(_inputName.Text);
        }
    }
}