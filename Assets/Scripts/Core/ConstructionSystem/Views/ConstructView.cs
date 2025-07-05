using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections.Generic;
using CastleFight.Extensions;
using Zenject;
using CastleFight.Core.Configs;

[RequireComponent(typeof(MeshRenderer))]
public class ConstructView : MonoBehaviour
{
    [Inject] private ConstructViewConfig _config;

    private Texture2D _gridTexture;
    private Material _materialInstance;
    private HashSet<Type> _obstacleTypes;

    private float _basePlaneSize;

    private void Start()
    {
        _basePlaneSize = _config.PlaneSize;

        CacheObstacleTypes();
        GenerateChessboardTexture();
        ApplyMaterial();
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

    private void LateUpdate()
    {
        if (!EventSystem.current.IsBlockedByUI())
        {
            MoveWithMouse();
            UpdateColorBasedOnObstacles();
        }
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

        _materialInstance.SetColor("_Color", targetColor);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(PlaneSize, 0.1f, PlaneSize));
    }
#endif
}
