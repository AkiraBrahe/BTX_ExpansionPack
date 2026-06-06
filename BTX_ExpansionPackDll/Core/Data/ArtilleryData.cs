using BattleTech;
using UnityEngine;

namespace BTX_ExpansionPack.Core.Data
{
    public class ArtilleryData
    {
        public enum ArtilleryTargetingMode
        {
            Single,
            Cluster,
            Barrage,
            CounterBattery
        }

        public struct ArtilleryStrikeData
        {
            public int Round { get; set; }
            public string TeamGUID { get; set; }
            public Vector3 TargetPosition { get; set; }
            public ArtilleryTargetingMode Mode { get; set; }
        }

        public struct TargetMovementData
        {
            public AbstractActor Target { get; set; }
            public Vector3 CurrentPos { get; set; }
            public Vector3 PredictedPos { get; set; }
            public Vector3 MoveVector { get; set; }
            public Vector3 ClosestAllyPos { get; set; }
        }
    }
}