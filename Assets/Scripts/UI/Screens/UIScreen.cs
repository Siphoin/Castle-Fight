using UnityEngine;
namespace CastleFight.UI.Screens
{
    public abstract class UIScreen : MonoBehaviour
    {
        public void SetStateVisible (bool visible) => gameObject.SetActive(visible);
    }
}
