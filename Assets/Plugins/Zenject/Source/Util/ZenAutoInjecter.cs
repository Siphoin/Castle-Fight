using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class ZenAutoInjecter : MonoBehaviour
    {
        bool _hasInjected;

        [Inject]
        public void Construct()
        {
            if (!_hasInjected)
            {
                throw Assert.CreateException(
                    "ZenAutoInjecter was injected! Do not use ZenAutoInjecter for objects that are instantiated through zenject or which exist in the initial scene hierarchy");
            }
        }

        public void OnEnable()
        {
            _hasInjected = true;
            var container = FindSuitableContainer();
            var sceneContext = GameObject.FindObjectOfType<SceneContext>();
            sceneContext.Container.Inject(gameObject);

            if (container != null)
            {
                container.InjectGameObject(gameObject);
            }
            else
            {
                Debug.LogWarning($"No Zenject container found for {gameObject.name}");
            }
        }

        private DiContainer FindSuitableContainer()
        {
            var parentContext = transform.GetComponentInParent<Context>();
            if (parentContext != null)
            {
                return parentContext.Container;
            }

            try
            {
                var sceneContainer = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>()
                    .GetContainerForScene(gameObject.scene);
                if (sceneContainer != null)
                {
                    return sceneContainer;
                }
            }
            catch
            {
            }

            return ProjectContext.Instance.Container;
        }
    }
}