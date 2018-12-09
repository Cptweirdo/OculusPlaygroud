using Buffers;
using Components;

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

namespace Systems
{
    [UpdateAfter(typeof(SpawnerSystem))]
    public class MovementSystem : JobComponentSystem
    {

#pragma warning disable 649
        /*
        struct SpawnedObjects
        {
            public ComponentDataArray<Position> Position;
            public ComponentDataArray<Rotation> Rotation;
            [ReadOnly]
            public ComponentDataArray<Scale> Scale;
            [ReadOnly]
            public ComponentDataArray<PositionRotation> PositionRotation;

            public EntityArray Entity;
            public readonly int Length;
        }

        [Inject] SpawnedObjects spawnedObjects;
                */
        struct Configs
        {
            [ReadOnly]
            public SharedComponentDataArray<Config> Config;
            public BufferArray<CombinedMeshVerticesBuffer> Buffer;
            public readonly int Length;
        }

        [Inject] Configs configs;
#pragma warning restore 649
        private struct MovementJob : IJobProcessComponentData<Position, PositionRotation, ObjectID>
        {
            [NativeDisableParallelForRestriction]
            public BufferArray<CombinedMeshVerticesBuffer> Buffers;
            public float3 repPos;
            public float dt, chargeRepulsor, springReturn, eps1, eps2;
            public int vertCount;
            public void Execute(
                ref Position currentPos,
                ref PositionRotation originaPosRot,
                ref ObjectID objectID)
            {

                var Buffer = Buffers[0];
                var pos = currentPos.Value;
                var ret = springReturn * (originaPosRot.Position - pos);

                var rep = pos - repPos;
                rep = rep / Mathf.Max(eps1, Vector3.SqrMagnitude(rep));

                ret += rep;
                if (math.length(ret) > eps2)
                {
                    ret = ret * dt;
                    currentPos = new Position() { Value = pos + ret };
                    for (int i = vertCount * objectID.Value; i < vertCount * (1 + objectID.Value); i++)
                    {
                        Buffer[i] = Buffer[i].Value + new Vector3(ret.x,ret.y,ret.z);
                    }
                    // EntityManager.AddComponent(spawnedObjects.Entity[i], typeof(DirtyComponent));
                }
            }
        }

        const float FRAME = 1f / 90f;
        public float chargeRepulsor = 1f;
        public float springReturn = 1f;
        public float eps1 = 0.01f;
        public float eps2 = 0.01f;

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            
            Config config = configs.Config[0];
            MovementJob job = new MovementJob() {
                dt = Time.deltaTime,
                chargeRepulsor = chargeRepulsor,
                springReturn = springReturn,
                eps1 = eps1,
                eps2 = eps2,
                repPos = config.repulsor.transform.position,
                vertCount = config.spawnedItemMesh.vertexCount,
                Buffers = configs.Buffer
            };
            return job.Schedule(this, inputDeps);
        }
    }
}

