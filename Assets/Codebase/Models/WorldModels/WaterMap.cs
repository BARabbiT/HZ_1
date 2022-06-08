using UnityEngine;

namespace Assets.Codebase.Models.WorldModels
{
    public class WaterMap
    {
        public Vector3[] Verticies { get; set; }
        public Triangle[] Triangles { get; set; }

        public int OceanOffset { get; set; }
        public float WaterLine { get; set; }
    }
}
