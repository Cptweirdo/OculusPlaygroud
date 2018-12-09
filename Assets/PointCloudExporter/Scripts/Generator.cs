using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace PointCloudExporter
{
    public class Generator: MonoBehaviour
    {
        public string pointCloudName;
        public GameObject particle;
        public GameObject combined;
        public int maxVertices = 100000;
        [Range(0,1)]
        public float scale = 0.1f;

        GameObject[] particles;

        public void Generate()
        {
            Assert.IsNotNull(particle, "No particle prefab specified");
            Assert.IsNotNull(particle.GetComponent<MeshFilter>().sharedMesh, "Particle has no mesh");
            Assert.IsNotNull(particle.GetComponent<MeshRenderer>(), "Particle has no mesh renderer");
            Assert.IsNotNull(pointCloudName, "No point cloud specified");
            Assert.IsTrue(maxVertices > 1, "incorrect number of Vertices");
            Assert.IsTrue(scale > 0, "Scale needs to be positive");

            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, pointCloudName) + ".ply";
            MeshInfos points = PLYImporter.Load(filePath, maxVertices);
            Assert.IsNotNull(points, "Point Cloud could not be loaded");
            GenerateParticles(points, particle);
        }

        private void GenerateParticles(MeshInfos points, GameObject particle)
        {
            int vertexCount = points.vertexCount;
            particles = new GameObject[vertexCount];
            CombineInstance[] combineInstances = new CombineInstance[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                particles[i] = Instantiate(
                    particle,
                    points.vertices[i],
                    Quaternion.FromToRotation(Vector3.forward, points.normals[i]),
                    transform);
                particles[i].transform.localScale = Vector3.one * scale;
                /*
                meshRenderer = particles[i].GetComponent<MeshRenderer>();
                MaterialPropertyBlock props = new MaterialPropertyBlock();
                props.SetColor("_Color", points.colors[i]);
                meshRenderer.SetPropertyBlock(props);
                */
                combineInstances[i].mesh = particles[i].GetComponent<MeshFilter>().sharedMesh;
                combineInstances[i].subMeshIndex = 0;
                Transform _transform = particles[i].transform;
                combineInstances[i].transform = Matrix4x4.TRS(_transform.position, _transform.rotation, _transform.localScale);
            }
            var _combined = Instantiate(
                    combined,
                    transform);
            var filter = _combined.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combineInstances);
            Debug.Log("DEBUG " + mesh.vertexCount);
            filter.sharedMesh = mesh;
            _combined.transform.position = Vector3.zero;
        }

        public void Reset()
        {
            List<GameObject> childred = new List<GameObject>();
            foreach (Transform child in transform)
            {
                childred.Add(child.gameObject);
            }

            foreach (var item in childred)
            {
                DestroyImmediate(item);
            }
        }
    }
}
