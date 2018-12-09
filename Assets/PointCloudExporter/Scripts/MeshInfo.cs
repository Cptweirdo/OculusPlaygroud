using Unity.Mathematics;

using UnityEngine;

namespace PointCloudExporter
{
    public class MeshInfos
    {
        public float3[] vertices;
        public float3[] normals;
        public Color[] colors;
        public int vertexCount;
        public Bounds bounds;
    }
}
