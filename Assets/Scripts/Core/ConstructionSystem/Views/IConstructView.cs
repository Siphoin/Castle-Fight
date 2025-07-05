using CastleFight.Core.BuildingsSystem;
using UnityEngine;

namespace CastleFight.Core.ConstructionSystem.Views
{
    public interface IConstructView
    {
        bool CanConstruct { get; }
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}