using System;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    /// <summary>
    /// Spawn count Entities based on the specified Prefab. Components on the Prefab will be added to the Entities.
    /// The PositionComponent of each Entity will be set to a random position on the circle described by
    /// the PositionComponent associated with this component and the radius.
    /// </summary>
    [Serializable]
    public struct Config : ISharedComponentData
    {
        public Mesh spawnedItemMesh;
        public Material spawnedItemMaterial;
        public GameObject spawnedItemPrefab;
        public int maxObjects;
        public float scale;
        public string name;
        public GameObject repulsor;
    }

    public class ConfigComponent : SharedComponentDataWrapper<Config> { }
}