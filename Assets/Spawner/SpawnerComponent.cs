using System;

using Unity.Entities;

namespace Components
{
    [Serializable]
    public struct Spawner : IComponentData
    {
    }

    public class SpawnerComponent : ComponentDataWrapper<Spawner> { }
}