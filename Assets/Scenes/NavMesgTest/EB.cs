using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB : MonoBehaviour
{
    public delegate void MeshDone();
    public event MeshDone MeshDone_Event;
    public void OnMeshDone()
    {
        if (MeshDone_Event != null) MeshDone_Event.Invoke();
    }

    public delegate void MeshUpdate(Vector3 updatePoint);
    public event MeshUpdate MeshUpdate_Event;
    public void OnMeshUpdate(Vector3 updatePoint)
    {
        if (MeshUpdate_Event != null) MeshUpdate_Event.Invoke(updatePoint);
    }
}
