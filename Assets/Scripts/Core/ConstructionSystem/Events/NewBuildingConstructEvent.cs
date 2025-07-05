using CastleFight.Core.BuildingsSystem;
using UnityEngine;

namespace CastleFight.Core.ConstructionSystem.Events
{
    public struct NewBuildingConstructEvent
    {

        public Vector3 Point { get; private set; }
        public IBuildingInstance Building { get; private set; }

        public NewBuildingConstructEvent(Vector3 point, IBuildingInstance building)
        {
            Point = point;
            Building = building;
        }
    }
}
