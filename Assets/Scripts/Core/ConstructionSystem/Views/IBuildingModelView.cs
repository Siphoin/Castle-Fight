using UnityEngine;

namespace CastleFight.Core.ConstructionSystem.Views
{
    public interface IBuildingModelView
    {
        Mesh Mesh { get; }
        Vector3 Scale { get; }
        Material Material { get; }
    }
}