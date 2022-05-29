using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRenderTexture1fProvider
{
    RenderTexture RenderTexture { get; }
}

public interface IRenderTexture4fProvider
{
    RenderTexture RenderTexture4f { get; }
}
