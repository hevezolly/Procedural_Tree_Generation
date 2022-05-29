using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "new marching cube mesh generator", menuName = "Visuals/marchingCubes")]
public class MarchingCubeMeshGenerator : ScriptableObject
{
    private const string TringleBufferName = "triangles";
    private const string ResolutionName = "resolution";
    private const string ChunkIndex = "chunkIndex";
    private const string TotalChunks = "totalChunks";
    private const string TextureName = "densiyTexture";
    private const string KernelName = "CSMain";
    private const string SurfaceLevelName = "surfaceLevel";
    private const string Smoothness = "smoothness";
    private const string SmoothShading = "smoothShading";
    private const string ColorTexture = "colorTexture";
    private const string NormalMap = "normalMap";

    private const int MaxTringles = 100; //21845;

    private readonly Vector3Int threadsNumber = new Vector3Int(8, 8, 1);

    [SerializeField]
    private bool drawGizmos;

    [SerializeField]
    private Vector3Int cubesPerAxisInChunk;
    [SerializeField]
    private Vector3Int chunksPerAxis;
    [SerializeField]
    private SurfaceLevel surfaceLevel;
    [SerializeField]
    [Range(0, 1)]
    private float smoothness;
    [SerializeField]
    private bool smoothShading;
    public float SurfaceLevel => surfaceLevel.Value;

    private struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 color;
    }

    private struct Triangle
    {
        public Vertex vertexC;
        public Vertex vertexB;
        public Vertex vertexA;
    }

    private ComputeBuffer CreateTringleBuffer()
    {
        int numVoxels = cubesPerAxisInChunk.x * cubesPerAxisInChunk.y * cubesPerAxisInChunk.z;
        int maxTriangleCount = numVoxels * 5;
        var vertexSize = sizeof(float) * 9;


        var buffer = new ComputeBuffer(maxTriangleCount, vertexSize * 3,
            ComputeBufferType.Append);
        buffer.SetCounterValue(0);
        return buffer;
    }

    private int GetProcessors(int thread, int number) => number / thread + (int)Mathf.Sign((number % thread));

    private int GetBufferSize(ComputeBuffer buffer)
    {
        var triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        int[] triCountData = new int[1];
        triCountBuffer.SetData(triCountData);
        ComputeBuffer.CopyCount(buffer, triCountBuffer, 0);

        triCountBuffer.GetData(triCountData);
        triCountBuffer.Dispose();
        return triCountData[0];
    }


    [SerializeField]
    private ComputeShader shader;
    public IEnumerator CalculateMesh(RenderTexture dencityTexture, RenderTexture colorTexture, RenderTexture normalMap, Action<Mesh[]> onCalculated)
    {
        var buffer = CreateTringleBuffer();
        var result = new List<Triangle>();
        for (var x = 0; x < chunksPerAxis.x; x++)
        {
            for (var y = 0; y < chunksPerAxis.y; y++)
            {
                for (var z = 0; z < chunksPerAxis.z; z++)
                {
                    var cord = new Vector3Int(x, y, z);
                    DispatchChunk(cord, dencityTexture, colorTexture, normalMap, buffer);
                    yield return new WaitForEndOfFrame();

                    var size = GetBufferSize(buffer);
                    var intermidiateResult = new Triangle[size];
                    buffer.GetData(intermidiateResult, 0, 0, size);
                    result.AddRange(intermidiateResult);
                    buffer.SetCounterValue(0);
                }
            }
        }

        //var request = AsyncGPUReadback.Request(buffer);

        //while (!request.done)
        //    yield return new WaitForEndOfFrame();

        buffer.Dispose();
        onCalculated?.Invoke(GetMeshes(result));
    }

    private void DispatchChunk(Vector3Int coordinate, RenderTexture densityTexture, RenderTexture colorTexture, RenderTexture normalMap, ComputeBuffer buffer)
    {
        var kernel = shader.FindKernel(KernelName);
        shader.SetBuffer(kernel, TringleBufferName, buffer);
        shader.SetTexture(kernel, ColorTexture, colorTexture);
        shader.SetInts(ResolutionName, cubesPerAxisInChunk.x, cubesPerAxisInChunk.y, cubesPerAxisInChunk.z);
        shader.SetInts(ChunkIndex, coordinate.x, coordinate.y, coordinate.z);
        shader.SetInts(TotalChunks, chunksPerAxis.x, chunksPerAxis.y, chunksPerAxis.z);
        shader.SetTexture(kernel, TextureName, densityTexture);
        shader.SetTexture(kernel, NormalMap, normalMap);
        shader.SetFloat(SurfaceLevelName, surfaceLevel.Value);
        shader.SetInt(SmoothShading, smoothShading ? 1 : 0);
        shader.SetFloat(Smoothness, smoothness);
        shader.Dispatch(kernel,
            GetProcessors(threadsNumber.x, cubesPerAxisInChunk.x),
            GetProcessors(threadsNumber.y, cubesPerAxisInChunk.y),
            GetProcessors(threadsNumber.z, cubesPerAxisInChunk.z));
    }

    private Color ColorFromVec(Vector3 vec)
    {
        return new Color(vec.x, vec.y, vec.z);
    }

    private Mesh[] GetSmoothMesh(IEnumerable<Triangle> calculatedTringles)
    {
        var verticesIndexed = new Dictionary<Vector3, int>();
        var vertices = new List<Vector3>();
        var tringles = new List<int>();
        var colors = new List<Color>();
        var normals = new List<Vector3>();
        foreach (var t in calculatedTringles)
        {

            var v1 = t.vertexA.position - Vector3.one / 2;
            var v2 = t.vertexB.position - Vector3.one / 2;
            var v3 = t.vertexC.position - Vector3.one / 2;

            if (!verticesIndexed.ContainsKey(v1))
            {
                verticesIndexed[v1] = vertices.Count;
                vertices.Add(v1);
                normals.Add(t.vertexA.normal);
                colors.Add(ColorFromVec(t.vertexA.color));
            }
            if (!verticesIndexed.ContainsKey(v2))
            {
                verticesIndexed[v2] = vertices.Count;
                vertices.Add(v2);
                normals.Add(t.vertexB.normal);
                colors.Add(ColorFromVec(t.vertexB.color));
            }
            if (!verticesIndexed.ContainsKey(v3))
            {
                verticesIndexed[v3] = vertices.Count;
                vertices.Add(v3);
                normals.Add(t.vertexC.normal);
                colors.Add(ColorFromVec(t.vertexC.color));
            }

            tringles.Add(verticesIndexed[v1]);
            tringles.Add(verticesIndexed[v2]);
            tringles.Add(verticesIndexed[v3]);
        }
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tringles, 0);
        mesh.SetColors(colors);
        Debug.Log("total vertices: " + mesh.vertices.Count());
        Debug.Log("total tringles: " + mesh.triangles.Count() / 3);
        return new Mesh[] { mesh };
    }

    private Mesh[] GetMeshes(IEnumerable<Triangle> calculatedTringles)
    {
        var vertices = new List<List<Vector3>>();
        var normals = new List<List<Vector3>>();
        var tringles = new List<List<int>>();
        var colors = new List<List<Color>>();
        var tringleCount = 0;
        var meshIndex = 0;
        foreach (var t in calculatedTringles)
        {
            if (tringleCount == 0)
            {
                vertices.Add(new List<Vector3>());
                normals.Add(new List<Vector3>());
                tringles.Add(new List<int>());
                colors.Add(new List<Color>());
            }

            tringles[meshIndex].Add(vertices[meshIndex].Count);
            tringles[meshIndex].Add(vertices[meshIndex].Count + 1);
            tringles[meshIndex].Add(vertices[meshIndex].Count + 2);

            vertices[meshIndex].Add(t.vertexA.position - Vector3.one / 2);
            vertices[meshIndex].Add(t.vertexB.position - Vector3.one / 2);
            vertices[meshIndex].Add(t.vertexC.position - Vector3.one / 2);

            normals[meshIndex].Add(t.vertexA.normal);
            normals[meshIndex].Add(t.vertexB.normal);
            normals[meshIndex].Add(t.vertexC.normal);

            
            colors[meshIndex].Add(ColorFromVec(t.vertexA.color));
            colors[meshIndex].Add(ColorFromVec(t.vertexB.color));
            colors[meshIndex].Add(ColorFromVec(t.vertexC.color));

            tringleCount++;
            if (tringleCount >= MaxTringles)
            {
                tringleCount = 0;
                meshIndex++;
            }
        }
        var meshes = new Mesh[vertices.Count];
        for (var i = 0; i < vertices.Count; i++)
        {
            meshes[i] = new Mesh();
            meshes[i].SetVertices(vertices[i]);
            meshes[i].SetNormals(normals[i]);
            meshes[i].SetTriangles(tringles[i], 0);
            meshes[i].SetColors(colors[i]);
        }
        return meshes;
    }

    public void OnGizmos(Transform transform)
    {
        if (!drawGizmos)
            return;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        var offset = new Vector3(1f / (cubesPerAxisInChunk.x * chunksPerAxis.x),
                        1f / (cubesPerAxisInChunk.y * chunksPerAxis.y),
                        1f / (cubesPerAxisInChunk.z * chunksPerAxis.z));
        var chunkOffset = new Vector3(1f / chunksPerAxis.x,
                        1f / chunksPerAxis.y,
                        1f / chunksPerAxis.z);
        var scale = Vector3.one;
        Gizmos.DrawWireCube(Vector3.zero, scale);
        Gizmos.DrawCube(Vector3.zero, offset);
        for (var x = 0; x < chunksPerAxis.x; x++)
        {
            for (var y = 0; y < chunksPerAxis.y; y++)
            {
                for (var z = 0; z < chunksPerAxis.z; z++)
                {
                    var pos = Vector3.Scale(chunkOffset, new Vector3(x, y, z)) - Vector3.one / 2 + chunkOffset / 2;
                    Gizmos.DrawWireCube(pos, chunkOffset);
                }
            }
        }
    }
}


