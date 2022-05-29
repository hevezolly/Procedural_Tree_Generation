using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureViewer3D4f : MonoBehaviour
{
	[Range(0, 1)]
	[SerializeField]
	private float sliceDepth;
	[SerializeField]
	private float surfaceLevel;
	[SerializeField]
	private bool showX;
	[SerializeField]
	private bool showY;
	[SerializeField]
	private bool showZ;
	[SerializeField]
	private bool showW;
	private Material material;
	[SerializeField]
	private GameObject rendertTexture4Provider;

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
		var tex = rendertTexture4Provider.GetComponent<IRenderTexture4fProvider>().RenderTexture4f;
		if (tex == null)
			return;
		material.SetFloat("sliceDepth", sliceDepth);
		material.SetTexture("DisplayTexture", tex);
		material.SetFloat("surfaceLevel", surfaceLevel);
		material.SetVector("layersToShow", new Vector4(
			showX ? 1f : 0,
			showY ? 1f : 0,
			showZ ? 1f : 0,
			showW ? 1f : 0));
	}
}
