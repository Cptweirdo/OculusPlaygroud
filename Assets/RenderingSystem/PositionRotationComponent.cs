using System;

using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [Serializable]
    public struct PositionRotation : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
    }

    public class PositionRotationComponent : ComponentDataWrapper<PositionRotation> { }
}