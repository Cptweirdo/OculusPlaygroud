using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Buffers
{
    public struct CombinedMeshVerticesBuffer : IBufferElementData
    {
        // These implicit conversions are optional, but can help reduce typing.
        public static implicit operator float3(CombinedMeshVerticesBuffer e) { return e.Value; }
        public static implicit operator CombinedMeshVerticesBuffer(float3 e) { return new CombinedMeshVerticesBuffer { Value = e }; }
        public static implicit operator Vector3(CombinedMeshVerticesBuffer e) { return e.Value; }
        public static implicit operator CombinedMeshVerticesBuffer(Vector3 e) { return new CombinedMeshVerticesBuffer { Value = e }; }

        public Vector3 Value;
    }
}