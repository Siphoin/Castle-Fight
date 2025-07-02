using UnityEngine;

namespace CastleFight.DevTools
{
    public class GizmosViewSphere : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private Color _color = Color.red;

        private void Start()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
