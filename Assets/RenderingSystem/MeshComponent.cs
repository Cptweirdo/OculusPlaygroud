using System;

using Unity.Entities;
using UnityEngine;

namespace Components
{
    [Serializable]
    public struct MeshData : ISharedComponentData
    {
        public Mesh Value;
    }

    public class MeshComponent : SharedComponentDataWrapper<MeshData> { }
}