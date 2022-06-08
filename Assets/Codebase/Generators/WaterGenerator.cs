///////////////////////////////////
/// author: Vohmyakov Andrey
///////////////////////////////////

using Assets.Codebase.Models.WorldModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Codebase.Generators
{
    /// <summary>
    /// Water generator
    /// </summary>
    public class WaterGenerator
    {
        readonly HashSet<Vector3> _terrainBeforeVerticies;
        readonly int _mapSize;
        readonly float _waterLine;
        readonly int _oceanOffset;

        public WaterGenerator(Vector3[] terrainBeforeVerticies, int mapSize, int oceanOffset, float waterLine)
        {
            _terrainBeforeVerticies = terrainBeforeVerticies.ToHashSet();
            _mapSize = mapSize + oceanOffset;
            _waterLine = waterLine;
            _oceanOffset = oceanOffset + mapSize;
        }
 
        public WaterMap Generate(Vector3[] newVerts)
        {
            WaterMap waterMap = new();

            Debug.Log("Start water gen");

            var Verticies = new Vector3[(_mapSize + 1) * (_mapSize + 1)];
            var Triangles = new Triangle[_mapSize * _mapSize * 2];

            int t = 0;
            int v = 0;
            for (int x = 0; x < _mapSize; x++)
            {
                for (int z = 0; z < _mapSize; z++)
                {
                    Triangle triangle1 = new Triangle()
                    {
                        I1 = v,
                        I2 = v + 1,
                        I3 = v + (_mapSize + 1)
                    };

                    Triangle triangle2 = new Triangle()
                    {
                        I1 = v + (_mapSize + 1),
                        I2 = v + 1,
                        I3 = v + (_mapSize + 1) + 1
                    };

                    Triangles[t] = triangle1;
                    Triangles[t+1] = triangle2;

                    v++;
                    t+=2;
                }
                v++;
            }

            v = 0;
            t = 0;
            for (int x = 0; x <= _mapSize; x++)
            {
                for (int z = 0; z <= _mapSize; z++)
                {
                    Verticies[v] = new Vector3(x , 0, z);
                    v++;
                }
            }

            for (int i=0; i < newVerts.Length;i++)
            {
                var nv = newVerts[i];
                var oldV = _terrainBeforeVerticies.First(ov=>ov.x == nv.x && ov.z == nv.z);
                var rivV = Verticies.First(v => v.x == oldV.x && v.z == oldV.z);
                int index = Array.IndexOf(Verticies, rivV);

                Verticies[index].y = oldV.y;

            }

            Debug.Log("End water gen");
            waterMap.Verticies = Verticies;
            waterMap.Triangles = Triangles;
            waterMap.WaterLine = _waterLine;
            waterMap.OceanOffset = _oceanOffset;

            return waterMap;
        }

    }
}
