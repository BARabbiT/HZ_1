using Assets.Codebase.Models.DataModels;
using Assets.Codebase.Models.WorldModels;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMap
{
    public Vector3[] Verticies { get; set; }
    public Vector3[] NormalisedVerticies { get; set; }
    public List<Triangle> Triangles { get; set; }

    public List<(GameObject, Vector3)> GameObjects { get; set; }


    public float DeltaMaxHeight { get; set; }
    public float DeltaMinHeight { get; set; }

    public float DeltaMaxAlphaHeight { get; set; }
    public float DeltaMinAlphaHeight { get; set; }

}


