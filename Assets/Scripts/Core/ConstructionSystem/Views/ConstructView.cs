using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections.Generic;
using CastleFight.Extensions;
using Zenject;
using CastleFight.Core.Configs;
using CastleFight.Core.BuildingsSystem;
using Core.ConstructionSystem.Handlers;
using UniRx;
namespace CastleFight.Core.ConstructionSystem.Views
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ConstructView : MonoBehaviour, IConstructView
    {
        [Inject] private ConstructViewConfig _config;

        private Texture2D _gridTexture;
        private Material _materialInstance;
        private HashSet<Type> _obstacleTypes;

        private IBuildingInstance _targetBuildng;

        private float _basePlaneSize;
        private GameObject _view;
        private MeshRenderer _viewRenderer;
        private MeshFilter _viewFitler;
        private ConstructHandler _handler;

        private static readonly Quaternion _defaultRotation = Quaternion.Euler(-90, 90, 0);

        public bool CanConstruct { get; private set; }
        public Vector3 Position => transform.position;
        public Quaternion Rotation => _view.transform.localRotation;

        public IConstructHandler Handler
        {
            get
            {
                if (!_handler)
                {
                    _handler = FindAnyObjectByType<ConstructHandler>();
                }
                return _handler;
            }
        }

        private void Awake()
        {
            Handler.OnSelectBuilding.Subscribe(building =>
            {
                gameObject.SetActive(true);
                SetBuilding(building);

            }).AddTo(this);

            Handler.OnEndBuild.Subscribe(endBuild =>
            {
                CanConstruct = false;
                gameObject.SetActive(false);

            }).AddTo(this);
        }

        private void Start()
        {
            _basePlaneSize = _config.PlaneSize;

            CacheObstacleTypes();
            GenerateChessboardTexture();
            ApplyMaterial();
            CreateView();

            gameObject.SetActive(false);
        }



        public float PlaneSize
        {
            get => _basePlaneSize * transform.localScale.x;
            set
            {
                if (value <= 0f) return;
                float scale = value / _basePlaneSize;
                transform.localScale = new Vector3(scale, transform.localScale.y, scale);
            }
        }

        private void LateUpdate()
        {
            if (!EventSystem.current.IsBlockedByUI())
            {
                MoveWithMouse();
                UpdateColorBasedOnObstacles();
            }
        }

        private void CacheObstacleTypes()
        {
            _obstacleTypes = new HashSet<Type>();

            foreach (var name in _config.ObstacleScriptNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                Type type = Type.GetType(name)
                    ?? AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == name);

                if (type != null && typeof(MonoBehaviour).IsAssignableFrom(type))
                    _obstacleTypes.Add(type);
                else
                    Debug.LogWarning($"Obstacle script type '{name}' not found or not a MonoBehaviour.");
            }
        }

        private void GenerateChessboardTexture()
        {
            _gridTexture = new Texture2D(_config.GridResolution, _config.GridResolution, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int x = 0; x < _config.GridResolution; x++)
            {
                for (int y = 0; y < _config.GridResolution; y++)
                {
                    bool isWhite = (x + y) % 2 == 0;
                    Color baseColor = isWhite ? Color.white : Color.black;
                    baseColor.a = 1f;
                    _gridTexture.SetPixel(x, y, baseColor);
                }
            }

            _gridTexture.Apply();
        }

        private void ApplyMaterial()
        {
            _materialInstance = new Material(_config.BaseMaterial);
            _materialInstance.SetTexture("_MaskTex", _gridTexture);
            var startColor = new Color(_config.GreenColor.r, _config.GreenColor.g, _config.GreenColor.b, _config.Opacity);
            _materialInstance.SetColor("_Color", startColor);

            var renderer = GetComponent<MeshRenderer>();
            renderer.material = _materialInstance;
        }


        private void MoveWithMouse()
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Vector3 pos = hit.point;
                pos.y += _config.YOffset;
                transform.position = pos;
            }
        }

        private void UpdateColorBasedOnObstacles()
        {
            Vector3 halfExtents = new Vector3(PlaneSize / 2f, 2f, PlaneSize / 2f);
            Collider[] colliders = Physics.OverlapBox(transform.position, halfExtents);

            bool hasObstacle = false;
            foreach (var col in colliders)
            {
                foreach (var type in _obstacleTypes)
                {
                    if (col.GetComponent(type) != null)
                    {
                        hasObstacle = true;
                        break;
                    }
                }
                if (hasObstacle) break;
            }

            Color targetColor = hasObstacle
                ? new Color(_config.RedColor.r, _config.RedColor.g, _config.RedColor.b, _config.Opacity)
                : new Color(_config.GreenColor.r, _config.GreenColor.g, _config.GreenColor.b, _config.Opacity);

            CanConstruct = hasObstacle == false;

            _materialInstance.SetColor("_Color", targetColor);
        }

        private void CreateView ()
        {
            _view = new GameObject("View");
            _viewRenderer = _view.AddComponent<MeshRenderer>();
            _viewFitler = _view.AddComponent<MeshFilter>();
            _view.transform.SetParent(transform, false);
            _view.transform.localPosition = Vector3.zero;
            _view.transform.localRotation = Quaternion.identity;
        }

        private void UpdateView()
        {
            _view.transform.localScale = _targetBuildng.BuildingView.Scale;
            _view.transform.rotation = _defaultRotation;
            if (_viewRenderer.material != null)
            {
                Destroy(_viewRenderer.material);
            }

            Material tempMaterial = new Material(_config.TransparentMaterial);
            Texture originalTexture = _targetBuildng.BuildingView.Material.mainTexture;
            tempMaterial.mainTexture = originalTexture;

            _viewRenderer.material = tempMaterial;
            _viewFitler.mesh = _targetBuildng.BuildingView.Mesh;
        }

        private void SetBuilding (IBuildingInstance building)
        {
            _targetBuildng = building;
            UpdateView();
        }
    }
}
