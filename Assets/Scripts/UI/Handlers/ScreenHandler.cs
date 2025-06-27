using System.Collections;
using CastleFight.Main;
using CastleFight.UI.Screens;
using UnityEngine;

namespace CastleFight.UI.Handlers
{
    public class ScreenHandler : MonoBehaviour, IScreenHandler
    {
        [SerializeField] private KeyValueList<ScreenType, UIScreen> _screens;
        [SerializeField] private ScreenType _currentScreen;

        private void Start()
        {
            foreach (var item in _screens)
            {
                var screen = item.Value;
                screen.SetStateVisible(false);
            }

            SetScreen(_currentScreen);
        }

        public void SetScreen (ScreenType screen)
        {
            var currentScreen = _screens[screen];
            currentScreen.SetStateVisible(false);
            _currentScreen = screen;
            var newScreen = _screens[screen];
            newScreen.SetStateVisible(true);

        }
    }
}