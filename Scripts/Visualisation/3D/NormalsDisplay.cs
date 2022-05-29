using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalsDisplay : MonoBehaviour
{
    private MeshFilter filter;
    [SerializeField]
    private bool onlySelected;
    [SerializeField]
    private bool drawNormals;
    [SerializeField]
    private bool drawShell;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
    }

    private void Draw()
    {
        if (filter == null || filter.mesh == null)
            return;
        if (drawNormals)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            for (var i = 0; i < filter.mesh.vertexCount; i++)
            {
                Gizmos.DrawLine(filter.mesh.vertices[i], filter.mesh.vertices[i] + filter.mesh.normals[i]);
            }
        }
        if (drawShell)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(filter.mesh, transform.position, transform.rotation, transform.lossyScale);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (onlySelected)
            Draw();
    }

    private void OnDrawGizmos()
    {
        if (!onlySelected)
            Draw();
    }
}
