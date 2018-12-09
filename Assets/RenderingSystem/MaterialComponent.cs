using System;

using Unity.Entities;
using UnityEngine;

namespace Components
{
    [Serializable]
    public struct MaterialData : ISharedComponentData
    {
        public Material Value;
    }

    public class MaterialComponent : SharedComponentDataWrapper<MaterialData> { }
}