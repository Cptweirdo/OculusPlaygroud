using PointCloudExporter;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Components;
using Buffers;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Rendering;

namespace Systems
{
    public class SpawnerSystem : ComponentSystem
    {

#pragma warning disable 649
        struct Group
        {
            [ReadOnly]
            public ComponentDataArray<Spawner> Spawner;
            [ReadOnly]
            public SharedComponentDataArray<Config> Config;

            public EntityArray Entity;
            public readonly int Length;
        }

        [Inject] Group m_Group;
#pragma warning restore 649

        protected override void OnUpdate()
        {
            while (m_Group.Length != 0)
            {
                Config spawner = m_Group.Config[0];

                Assert.IsNotNull(spawner.spawnedItemPrefab);
                Assert.IsNotNull(spawner.spawnedItemMesh);

                Entity sourceEntity = m_Group.Entity[0];

                MeshInfos points = PLYImporter.Load(
                    System.IO.Path.Combine(Application.streamingAssetsPath, spawner.name) + ".ply",
                    spawner.maxObjects
                    );
                int _objectCount = points.vertexCount;
                Debug.LogFormat("Spawning {0} objects", _objectCount);

                NativeArray<Entity> entities = new NativeArray<Entity>(_objectCount, Allocator.Temp);
                EntityManager.Instantiate(spawner.spawnedItemPrefab, entities);
                
                float3 pos;
                Quaternion rot;
                float3 scale = new float3(spawner.scale);
                Material mat = spawner.spawnedItemMaterial;
                //mat.SetColor("_Color", Color.red);
                var colors = new Color[spawner.spawnedItemMesh.vertexCount];
                NativeArray<CombineInstance> combineInstances = new NativeArray<CombineInstance>(_objectCount, Allocator.Temp);

                for (int i = 0; i < _objectCount; i++)
                {
                    pos = points.vertices[i];
                    rot = Quaternion.FromToRotation(Vector3.forward, points.normals[i]);
                    EntityManager.SetComponentData(entities[i], new Position
                    {
                        Value = pos
                    });
                    EntityManager.SetComponentData(entities[i], new Rotation
                    {
                        Value = rot
                    });
                    EntityManager.SetComponentData(entities[i], new PositionRotation
                    {
                        Position = pos,
                        Rotation = rot
                    });
                    EntityManager.SetComponentData(entities[i], new Scale
                    {
                        Value = scale
                    });
                    EntityManager.SetComponentData(entities[i], new ObjectID
                    {
                        Value = i
                    });

                    CombineInstance combineInstance = new CombineInstance();
                    Mesh _mesh = Mesh.Instantiate(spawner.spawnedItemMesh);
                    for (int j = 0; j < _mesh.vertexCount; j++)
                    {
                        colors[j] = points.colors[i];
                    }
                    _mesh.colors = colors;
                    combineInstance.mesh = _mesh;
                    combineInstance.transform = Matrix4x4.TRS(
                        pos,
                        rot,
                        scale
                        );
                    combineInstances[i] = combineInstance;
                    //EntityManager.AddComponent(entities[i],  typeof(DirtyComponent));
                }
                Mesh mesh = new Mesh();
                mesh.MarkDynamic();
                mesh.CombineMeshes(combineInstances.ToArray());
                var verts = new NativeArray<CombinedMeshVerticesBuffer>(mesh.vertexCount, Allocator.Temp);
                var vertsVector3 = mesh.vertices;
                for (int j = 0; j < verts.Length; j++)
                {
                    verts[j] = vertsVector3[j];
                }
                EntityManager.AddBuffer<CombinedMeshVerticesBuffer>(sourceEntity);
                var buffer = EntityManager.GetBuffer<CombinedMeshVerticesBuffer>(sourceEntity);
                buffer.AddRange(verts);

                EntityManager.AddSharedComponentData(sourceEntity, new MeshData()
                {
                    Value = mesh
                });

                EntityManager.AddSharedComponentData(sourceEntity, new MaterialData()
                {
                    Value = mat
                });

                EntityManager.RemoveComponent<Spawner>(sourceEntity);

                entities.Dispose();
                combineInstances.Dispose();
                verts.Dispose();

                // Instantiate & AddComponent & RemoveComponent calls invalidate the injected groups,
                // so before we get to the next spawner we have to reinject them  
                UpdateInjectedComponentGroups();
            }
        }
    }
}
