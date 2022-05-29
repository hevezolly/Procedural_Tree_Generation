using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class StraightForwardBranchGenerator : SinglePartGenerator
{
    [SerializeField]
    private GameObject MeshFragment;
    [SerializeField]
    private GameObject BranchesProvider;

    [SerializeField]
    [Min(0)]
    private int intermidiateCircles;

    [SerializeField]
    [Min(3)]
    private int branchCircleResolution=3;

    [SerializeField]
    [Min(0)]
    private int hamisphereVerticalResolution;

    [SerializeField]
    private int maxBranchesInBunch;

    private IBranchProvider branchProvider;

    private List<Tringle> tringlesToDisplay = new List<Tringle>();

    private void Awake()
    {
        branchProvider = BranchesProvider.GetComponent<IBranchProvider>();
    }
    public override void ClearMesh()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public override void GeneratePartsMesh()
    {
        ClearMesh();   
        foreach (var branch in GetBranches())
        {
            InstanciateSingleBranch(branch);
        }
    }

    private void InstanciateSingleBranch(Branch branch)
    {
        var mesh = GenerateSingleBranch(branch);
        var obj = Instantiate(MeshFragment, transform);
        obj.GetComponent<MeshFilter>().mesh = mesh;
    }

    public override IEnumerator StartGenerating()
    {
        ClearMesh();
        var bunches = GetBranches().Chunk(maxBranchesInBunch);
        foreach (var bunch in bunches)
        {
            foreach (var branch in bunch)
            {
                InstanciateSingleBranch(branch);
            }
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    private IEnumerable<Branch> GetBranches()
    {
        return branchProvider.GetBranches().Select(b => new Branch(b.start - Vector3.one / 2, b.end - Vector3.one / 2, b.color, b.startWidth, b.endWidth));
    }

    private Mesh GenerateSingleBranch(Branch branch)
    {
        var mesh = new Mesh();
        var vertexIndex = 0;
        var normal = (branch.end - branch.start).normalized;
        var tringles = new List<Tringle>();
        var vertices = new List<Vertex>();
        var startPoint = new Vertex()
        {
            position = branch.start - normal * branch.startWidth,
            normal = -normal,
            color = branch.color,
            index = vertexIndex++
        };
        vertices.Add(startPoint);
        var startOffset = GetOffset(normal);
        var startCircle = GetCircle(branch.start, startOffset * branch.startWidth, normal, branch.color, () => vertexIndex++).ToList();
        vertices.AddRange(startCircle);

        var startHemispher = GetHemisphere(startPoint, startCircle, branch.start, branch.color, () => vertexIndex++, true);
        tringles.AddRange(startHemispher.Item2);
        vertices.AddRange(startHemispher.Item1);

        for (var i = 0; i < intermidiateCircles; i++)
        {
            var t = (i + 1f) / (intermidiateCircles+1f);
            var pos = Vector3.Lerp(branch.start, branch.end, t);
            var c = GetCircle(pos, startOffset * Mathf.Lerp(branch.startWidth, branch.endWidth, t), normal, branch.color, () => vertexIndex++).ToList();
            vertices.AddRange(c);
            tringles.AddRange(ConnectTwoCircles(startCircle, c));
            startCircle = c;
        }

        var endPoint = new Vertex()
        {
            position = branch.end + normal * branch.endWidth,
            normal = normal,
            color = branch.color,
            index = vertexIndex++
        };
        vertices.Add(endPoint);
        var endCircle = GetCircle(branch.end, startOffset * branch.endWidth, normal, branch.color, () => vertexIndex++).ToList();
        vertices.AddRange(endCircle);
        var endHemispher = GetHemisphere(endPoint, endCircle, branch.end, branch.color, () => vertexIndex++);
        tringles.AddRange(endHemispher.Item2);
        vertices.AddRange(endHemispher.Item1);

        tringles.AddRange(ConnectTwoCircles(startCircle, endCircle));

        mesh.vertices = vertices.Select(v => v.position).ToArray();
        mesh.normals = vertices.Select(v => v.normal).ToArray();
        mesh.colors = vertices.Select(v => v.color).ToArray();
        mesh.triangles = tringles.SelectMany(t => t.Replersentation()).ToArray();
        tringlesToDisplay = tringles;
        return mesh;
    }

    private Tuple<List<Vertex>, List<Tringle>> GetHemisphere(Vertex point, List<Vertex> circle, Vector3 center, Color color, Func<int> getIndex, bool flipNormal=false)
    {
        var offset = circle.First().position - center;
        var up = (point.position - center).normalized;
        var rotationAxis = Vector3.Cross(offset.normalized, up).normalized;
        var stepAngle = 90f / (hamisphereVerticalResolution + 1);
        var rotator = Quaternion.AngleAxis(stepAngle, rotationAxis);
        var vertices = new List<Vertex>();
        var tringles = new List<Tringle>();
        offset = rotator * offset;
        var lastCirkle = circle;

        for (var i = 0; i < hamisphereVerticalResolution; i++)
        {
            var verticalOffset = Vector3.Project(offset, up);
            var newCenter = center + verticalOffset;
            var sideOffset = offset - verticalOffset;

            var cirkle = GetCircle(newCenter, sideOffset, up, color, getIndex, center).ToList();
            vertices.AddRange(cirkle);
            tringles.AddRange(ConnectTwoCircles(lastCirkle, cirkle, i==0 && flipNormal));
            lastCirkle = cirkle;
            offset = rotator * offset;
        }

        tringles.AddRange(ConnectCircleToVertex(lastCirkle, point));
        return new Tuple<List<Vertex>, List<Tringle>>(vertices, tringles);
    }

    private IEnumerable<Vector3> GetCirclePositions(Vector3 startOffset, Vector3 normal)
    {
        var offset = startOffset;
        var rotator = Quaternion.AngleAxis(360f / branchCircleResolution, normal);
        for (var i = 0; i < branchCircleResolution; i++)
        {
            yield return offset;
            offset = rotator * offset;
        }
        yield return startOffset;
    }

    private Vector3 GetOffset(Vector3 circleNormal)
    {
        var startOffset = Vector3.Cross(circleNormal, Vector3.forward);
        if (Mathf.Approximately(startOffset.sqrMagnitude, 0))
            startOffset = Vector3.Cross(circleNormal, Vector3.right);
        return startOffset.normalized;
    }

    private IEnumerable<Vertex> GetCircle(Vector3 position, Vector3 startOffset, Vector3 cirkleNormal, Color color, Func<int> getIndex, Vector3? sphereCenter = null)
    {
        var center = (sphereCenter == null) ? position : sphereCenter.Value;

        return GetCirclePositions(startOffset, cirkleNormal).Select(p => new Vertex()
        {
            position = position + p,
            normal = (position + p - center).normalized,
            color = color,
            index = getIndex()
        });
    }

    private IEnumerable<Tringle> ConnectTwoCircles(IEnumerable<Vertex> circle1, IEnumerable<Vertex> circle2, bool reverse=false)
    {
        var initial = circle1;
        if (reverse)
            initial = circle1.Reverse();
        var vertexPair = initial.Zip(circle2, (p1, p2) => new Tuple<Vertex, Vertex>(p1, p2));
        var previus = vertexPair.First();
        foreach (var current in vertexPair.Skip(1))
        {
            yield return new Tringle(previus.Item2, previus.Item1, current.Item1);
            yield return new Tringle(current.Item2, previus.Item2, current.Item1);
            previus = current;
        }
    }

    private IEnumerable<Tringle> ConnectCircleToVertex(IEnumerable<Vertex> cirkle, Vertex center)
    {
        var previus = cirkle.First();
        foreach (var cirkleVertex in cirkle.Skip(1))
        {
            yield return new Tringle(center, previus, cirkleVertex);
            previus = cirkleVertex;
        }
    }

    private class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Color color;
        public int index;

        public Vertex Move(Vector3 offset)
        {
            return new Vertex()
            {
                position = position + offset,
                normal = normal,
                color = color,
                index = index
            };
        }
    }

    private class Tringle
    {
        private Vertex[] vertices;

        public Tringle(Vertex v1, Vertex v2, Vertex v3)
        {
            vertices = new Vertex[] { v1, v2, v3 };
        }

        public IEnumerable<Vertex> Vertices()
        {
            foreach (var v in vertices)
            {
                yield return v;
            }
        }

        public IEnumerable<int> Replersentation()
        {
            return Vertices().Select(v => v.index);
        }
    }
}
