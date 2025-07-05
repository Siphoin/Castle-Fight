using System.Collections.Generic;
using CastleFight.Main.Configs;
using UnityEngine;

namespace CastleFight.Core.Configs
{
    [CreateAssetMenu(menuName = "Core/Configs/Construct View Config")]
    public class ConstructViewConfig : ScriptableConfig
    {
        [Header("Grid Settings")]
        [SerializeField, Min(1)] private int _gridResolution = 8;
        [SerializeField] private float _planeSize = 5f;
        [SerializeField] private float _yOffset = 0.1f;

        [Header("Materials and Obstacles")]
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private List<string> _obstacleScriptNames = new();

        [SerializeField] private Material _transparentMaterial;


        [Header("Colors and Opacity")]
        [SerializeField] private Color _greenColor = new Color(0f, 1f, 0f, 1f);
        [SerializeField] private Color _redColor = new Color(1f, 0f, 0f, 1f);
        [Range(0f, 1f)]
        [SerializeField] private float _opacity = 0.5f;

        public int GridResolution => _gridResolution;
        public float PlaneSize => _planeSize;
        public float YOffset => _yOffset;
        public Material BaseMaterial => _baseMaterial;
        public List<string> ObstacleScriptNames => _obstacleScriptNames;
        public Color GreenColor => _greenColor;
        public Color RedColor => _redColor;
        public float Opacity => _opacity;

        public Material TransparentMaterial => _transparentMaterial;
    }
}
