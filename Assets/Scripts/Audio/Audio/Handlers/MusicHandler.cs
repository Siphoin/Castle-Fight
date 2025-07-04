using UnityEngine;

namespace CastleFight.Audio
{
    public class MusicHandler : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
