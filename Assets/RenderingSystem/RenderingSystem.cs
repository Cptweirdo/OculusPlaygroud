using Buffers;
using Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

using UnityEngine;

namespace Systems
{
    // [UpdateAfter(typeof(SpawnerSystem))]
    public class RenderingSystem : ComponentSystem
    {

#pragma warning disable 649
        struct Configs
        {
            [ReadOnly]
            public SharedComponentDataArray<Config> Config;
            [ReadOnly]
            public SharedComponentDataArray<MeshData> MeshData;
            [ReadOnly]
            public SharedComponentDataArray<MaterialData> MaterialData;
            public BufferArray<CombinedMeshVerticesBuffer> Buffer;
            public EntityArray Entity;
            public readonly int Length;
        }

        [Inject] Configs configs;
#pragma warning restore 649

        const float FRAME = 1f / 90f;

        protected override void OnUpdate()
        {
            if (configs.Length > 0)
            {
                var MeshData = configs.MeshData[0];
                var mesh = MeshData.Value;

                var _verts = configs.Buffer[0].Reinterpret<Vector3>();
                var mat = configs.MaterialData[0].Value;
                /*
                var verts = mesh.vertices;
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i] = _verts[i];
                }
                 mesh.vertices = verts;
                 */
                mesh.vertices = _verts.ToNativeArray().ToArray(); // around 50%!

                PostUpdateCommands.SetSharedComponent(configs.Entity[0], new MeshData()
                {
                    Value = mesh
                });
                Graphics.DrawMesh(mesh, Matrix4x4.identity, mat, 0);
            }
        }
    }
}
