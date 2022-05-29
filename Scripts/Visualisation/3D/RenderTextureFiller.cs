using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "new render texture filler", menuName = "Visuals/render texture filler")]
public class RenderTextureFiller : ScriptableObject
{
    private const string TextureName = "renderTexture";

    [SerializeField]
    private string KernelName;
    [SerializeField]
    private Vector3Int threadNumber;
    [SerializeField]
    private ComputeShader shader;

    private int kernel;

    public RenderTextureFiller Init()
    {
        kernel = shader.FindKernel(KernelName);
        return this;
    }

    public RenderTextureFiller SetFloat(string name, float value)
    {
        shader.SetFloat(name, value);
        return this;
    }

    public RenderTextureFiller SetFloats(string name, params float[] values)
    {
        shader.SetFloats(name, values);
        return this;
    }

    public RenderTextureFiller SetInts(string name, params int[] values)
    {
        shader.SetInts(name, values);
        return this;
    }

    public RenderTextureFiller SetInt(string name, int value)
    {
        shader.SetInt(name, value);
        return this;
    }

    public RenderTextureFiller SetTexture(string name, Texture texture)
    {
        shader.SetTexture(kernel, name, texture);
        return this;
    }

    public RenderTextureFiller SetBuffer(ComputeBuffer buffer, string name)
    {
        shader.SetBuffer(kernel, name, buffer);
        return this;
    }

    public RenderTextureFiller SetBuffer(int maxElementsNumber, int elementSize, ComputeBufferType type, string name)
    {
        var buffer = new ComputeBuffer(maxElementsNumber, elementSize, type);
        shader.SetBuffer(kernel, name, buffer);
        return this;
    }

    public void FillTexture(ref RenderTexture value)
    {
        shader.SetTexture(kernel, TextureName, value);
        shader.Dispatch(kernel, value.width / threadNumber.x + 1, value.height / threadNumber.y + 1, value.volumeDepth / threadNumber.z + 1);
    }
}
