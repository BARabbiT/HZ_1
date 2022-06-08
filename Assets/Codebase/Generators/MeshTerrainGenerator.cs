///////////////////////////////////
/// author: Vohmyakov Andrey
///////////////////////////////////

using Assets.Codebase.Interfaces;
using Assets.Codebase.Models.DataModels;
using Assets.Codebase.Models.WorldModels;
using Assets.Codebase.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Codebase.Generators
{

    /// <summary>
    /// Basic algoritm terrain generation
    /// </summary>
    public static class MeshTerrainGenerator
    {

        public static TerrainMap CreateMeshShapeByNoiseType_Terrain(int mapSize, INoise noise, List<BiomeData> biomesData, int seed)
        {
            FastRandom Random = new(seed);
            TerrainMap meshMap = new TerrainMap
            {
                Triangles = new(),
                Verticies = new Vector3[(mapSize + 1) * (mapSize + 1)],
                NormalisedVerticies = new Vector3[(mapSize + 1) * (mapSize + 1)],
                GameObjects = new()
            };


            int v = 0;
            for (int x = 0; x < mapSize; x++)
            {
                for (int z = 0; z < mapSize; z++)
                {
                    Triangle triangle1 = new Triangle()
                    {
                        I1 = v,
                        I2 = v + 1,
                        I3 = v + (mapSize + 1)
                    };

                    Triangle triangle2 = new Triangle()
                    {
                        I1 = v + (mapSize + 1),
                        I2 = v + 1,
                        I3 = v + (mapSize + 1) + 1
                    };

                    meshMap.Triangles.Add(triangle1);
                    meshMap.Triangles.Add(triangle2);

                    v++;
                }
                v++;
            }

            v = 0;
            int iterator = 0;
            for (int x = 0; x <= mapSize; x++)
            {
                for (int z = 0; z <= mapSize; z++)
                {
                    float noiseHeight = noise.GetHeight(x, z, out float clearNoiseHeight, out bool isBorder);

                    Vector3 vector = new Vector3(x, noiseHeight, z);
                    Vector3 vectorN = new Vector3(x, clearNoiseHeight, z);

                    if (noiseHeight > meshMap.DeltaMaxHeight)
                        meshMap.DeltaMaxHeight = noiseHeight;
                    if (noiseHeight < meshMap.DeltaMinHeight)
                        meshMap.DeltaMinHeight = noiseHeight;

                    if (clearNoiseHeight > meshMap.DeltaMaxAlphaHeight)
                        meshMap.DeltaMaxAlphaHeight = clearNoiseHeight;
                    if (clearNoiseHeight < meshMap.DeltaMinAlphaHeight)
                        meshMap.DeltaMinAlphaHeight = clearNoiseHeight;

                    meshMap.NormalisedVerticies[v] = vectorN;
                    meshMap.Verticies[v] = vector;

                    if (iterator > 18)
                    {
                        var listObjectInst = ObjectsSpawn.NewWorldObjectSpawn(biomesData, seed, Random, vector, vectorN);
                        if (listObjectInst != default)
                            meshMap.GameObjects.AddRange(listObjectInst);
                        iterator = 0;
                    }
                    else
                    {
                        iterator++;
                    }
                    

                    v++;
                }
            }

            return meshMap;
        }

    }
}
