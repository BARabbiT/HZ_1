using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class MeshWorker : MonoBehaviour
{
    public List<NavMeshSurface> _surfaces = new List<NavMeshSurface>();
 
    public void Awake()
    {
        //SurfaceGenerator();
        GenerateNamMesh();
    }

    private void SurfaceGenerator()
    {
        for (int x = 0; x < gameObject.transform.localScale.x; x++)
        {
            for (int z = 0; z < gameObject.transform.localScale.z; z++)
            {
                NavMeshSurface surface = gameObject.AddComponent(typeof(NavMeshSurface)) as NavMeshSurface;
                surface.collectObjects = CollectObjects.Volume;
                surface.size = new Vector3(10f, 1f, 10f);
                if (z < x)
                {
                    surface.center = new Vector3(5, 0, z);
                }
                else
                {
                    surface.center = new Vector3(x, 0, z);
                }
                _surfaces.Add(surface);
            }
        }
        GenerateNamMesh();
    }

    private void GenerateNamMesh()
    {
        for (int i = 0; i < _surfaces.Count; i++)
        {
            _surfaces[i].BuildNavMesh();
        }
    }

    private void UpdateMesh(Vector3 point)
    {
        
    }
}
