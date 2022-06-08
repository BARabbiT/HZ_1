using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerate : MonoBehaviour
{
    private EB _eventBridge;
    Mesh _mesh;

    TerrainMap _meshMap;
    MeshFilter _meshFilter;
    MeshCollider _meshCollaider;
    MeshRenderer _meshRenderer;
    Transform _transform;

    private void Awake()
    {
        _eventBridge = gameObject.GetComponent<EB>();
        GenerateMesh();
    }
    private void GenerateMesh()
    {
        int xSize = 5;
        int zSize = 5;
        int v = 0;
        _meshFilter.mesh = _mesh;
        _mesh.vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        _mesh.triangles = new int[] {};
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
               // _mesh.triangles.
            }
        }
        _eventBridge.OnMeshDone();
    }
}
