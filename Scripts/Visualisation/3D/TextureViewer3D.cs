using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureViewer3D : MonoBehaviour
{

	[Range(0, 1)]
	public float sliceDepth;

	private Material material;
	[SerializeField]
	private SurfaceLevel surfaceLevel;
	[SerializeField]
	private GameObject rendertTexture1Provider;

	void Start()
	{

		material = GetComponentInChildren<MeshRenderer>().material;
		//
	}

	public void Display()
	{

	}


	void Update()
	{
		var tex = rendertTexture1Provider.GetComponent<IRenderTexture1fProvider>().RenderTexture;
		if (tex == null)
			return;
		material.SetFloat("sliceDepth", sliceDepth);
		material.SetTexture("DisplayTexture", tex);
		material.SetFloat("surfaceLevel", surfaceLevel.Value);
	}
}
