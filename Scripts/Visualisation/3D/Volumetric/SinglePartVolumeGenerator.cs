using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class SinglePartVolumeGenerator<T> : SinglePartGenerator, IRenderTexture4fProvider, IRenderTexture1fProvider
    where T: struct
{

    [SerializeField]
    private RenderTextureFiller partTextureFiller;

    [SerializeField]
    private RenderTextureFiller branchesToDensity;

    [SerializeField]
    private TextureCreator renderTextureCreator;

    [SerializeField]
    private SurfaceLevel surface;

    [SerializeField]
    private MarchingCubeMeshGenerator generator;
    [SerializeField]
    private GameObject chunkObj;

    [SerializeField]
    private int maxBranchesPerCall = 20;

    private RenderTexture branchesTexture;

    private RenderTexture finalTexture;

    private RenderTexture normalMap;
    public RenderTexture RenderTexture4f => branchesTexture;

    public RenderTexture RenderTexture => finalTexture;

    private Coroutine generateMeshCorutine;

    private RenderTexture colorTexture;

    protected virtual void Awake()
    {
        branchesTexture = renderTextureCreator.Create3DTextureFloat4();
        colorTexture = renderTextureCreator.Create3DTextureFloat4();
        normalMap = renderTextureCreator.Create3DTextureFloat4();
        //finalTexture = renderTextureCreator.Create3DTextureFloat1();
    }

    protected abstract T[] GetParts();

    public override IEnumerator StartGenerating()
    {
        if (branchesTexture == null)
            branchesTexture = renderTextureCreator.Create3DTextureFloat4();

        var branches = GetParts();
        if (branches.Length == 0)
            yield break;

        var useTex = false;
        var offset = 0;
        var buffer = GetPartsBuffer(branches);
        partTextureFiller.Init()
            .SetBuffer(buffer, "parts");
        var total = branches.Length / maxBranchesPerCall;
        for (var i = 0; i < branches.Length / maxBranchesPerCall; i++)
        {
            FillBranches(partTextureFiller, useTex, offset, maxBranchesPerCall);
            yield return new WaitForEndOfFrame();
            Debug.Log($"{i / (float)total * 100}%");
            useTex = true;
            offset += maxBranchesPerCall;
        }
        if (offset < branches.Length)
        {
            FillBranches(partTextureFiller, useTex, offset, branches.Length - offset);
            yield return new WaitForEndOfFrame();
        }
        buffer.Dispose();
        yield return GetMeshGeneratorCoroutine();
        generateMeshCorutine = null;
    }

    protected abstract ComputeBuffer GetPartsBuffer(T[] parts, ComputeBufferType type = ComputeBufferType.Default);

    protected virtual RenderTextureFiller SetAdditionalTextureFillerParams(RenderTextureFiller partTextureFiller)
    {
        return partTextureFiller;
    }

    private void FillBranches(RenderTextureFiller filler, bool useTextureAsInput, int indexOffset, int count)
    {
        SetAdditionalTextureFillerParams(filler)
            .SetInt("partIndexOffset", indexOffset)
            .SetInt("useTextureAsInput", useTextureAsInput ? 1 : 0)
            .SetFloat("surfaceLevel", surface.Value)
            .SetInt("numberOfParts", count)
            .FillTexture(ref branchesTexture);
    }

    private void GenerateMesh(Mesh[] meshes)
    {
        ClearMesh();
        foreach (var mesh in meshes)
        {
            var chunk = Instantiate(chunkObj, transform);
            var filter = chunk.GetComponent<MeshFilter>();
            filter.mesh = mesh;
        }
        generateMeshCorutine = null;
    }

    public override void GeneratePartsMesh()
    {
        if (generateMeshCorutine != null)
            StopCoroutine(generateMeshCorutine);
        generateMeshCorutine = StartCoroutine(StartGenerating());
    }

    public override void ClearMesh()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var c = transform.GetChild(i);
            c.parent = null;
            Destroy(c.gameObject);
        }
    }

    protected RenderTextureFiller SetAdditionalDensityMapperParams(RenderTextureFiller densityMapper)
    {
        return densityMapper;
    }

    private IEnumerator GetMeshGeneratorCoroutine()
    {
        if (finalTexture == null)
        {
            finalTexture = renderTextureCreator.Create3DTextureFloat1();
        }
        var brunches = GetPartsBuffer(GetParts());
        SetAdditionalDensityMapperParams(branchesToDensity.Init())
            .SetBuffer(brunches, "parts")
            .SetFloat("surfaceLevel", surface.Value)
            .SetTexture("colorTexture", colorTexture)
            .SetTexture("partsTexture", branchesTexture)
            .SetTexture("normalMap", normalMap)
            .FillTexture(ref finalTexture);
        brunches.Dispose();
        return generator.CalculateMesh(finalTexture, colorTexture, normalMap, GenerateMesh);
    }

    private void OnDrawGizmos()
    {
        if (generator == null)
            return;
        generator.OnGizmos(transform);
    }
}
