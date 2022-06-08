using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class NavMeshBaker : MonoBehaviour
{
    public List<NavMeshSurface> surfaces;
    private Assets.Codebase.Services.EventBridge _eventBridge;

    [Inject]
    public void InjectHandler(Assets.Codebase.Services.EventBridge eventBridge)
    {
        _eventBridge = eventBridge;
    }
    public void Awake()
    {
        _eventBridge.WorldPreFinilised_Event += GenerateNavMesh;
    }

    public void OnDestroy()
    {
        _eventBridge.WorldPreFinilised_Event -= GenerateNavMesh;
    }

    private void GenerateNavMesh()
    {
        Debug.Log("Start creating navMesh");
        for (int i = 0; i < surfaces.Count; i++)
        {
            surfaces[i].BuildNavMesh();
        }
        Debug.Log("Finished creating navMesh");

    }

    public void AddSurfaceToGenerateNavMesh(NavMeshSurface surface)
    {
        surfaces.Add(surface);
    }
}
