using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new texture creator", menuName = "Visuals/Texture Creator")]
public class TextureCreator : ScriptableObject
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private int size;

    private RenderTexture CreateTexture(UnityEngine.Experimental.Rendering.GraphicsFormat format)
    {
        const int numBitsInDepthBuffer = 0;
        var texture = new RenderTexture(size, size, numBitsInDepthBuffer);
        texture.graphicsFormat = format;
        texture.volumeDepth = size;
        texture.enableRandomWrite = true;
        texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;

        texture.Create();
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Trilinear;
        texture.name = Name;
        return texture;
    }

    public RenderTexture Create3DTextureFloat1()
    {
        return CreateTexture(UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat);   
    }

    public RenderTexture Create3DTextureFloat4()
    {
        return CreateTexture(UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat);
    }
}
