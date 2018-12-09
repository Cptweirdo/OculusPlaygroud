using System;

using Unity.Entities;

namespace Components
{
    [Serializable]
    public struct ObjectID : IComponentData
    {
        public int Value;
    }

    public class IDComponent : ComponentDataWrapper<ObjectID> { }
}