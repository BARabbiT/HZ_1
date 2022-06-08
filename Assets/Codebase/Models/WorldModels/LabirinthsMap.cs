using Assets.Codebase.Models.DataModels;
using Assets.Codebase.Models.WorldModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabirinthsMap
{
    public Vector3[] Verticies { get; set; }
    public List<int> Triangles { get; set; }
    public LabirinthData Labirinths { get; set; }
}


