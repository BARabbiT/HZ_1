using Assets.Codebase.Models.DataModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Codebase.Utils
{
    public static class ObjectsSpawn
    {
        public static List<(GameObject, Vector3)> NewWorldObjectSpawn(List<BiomeData> biomesData,int seed, FastRandom Random, Vector3 vertex, Vector3 vertexN)
        {
            List<(GameObject, Vector3)> listObj = new();

            var biom = BiomeUtils.GetBiome(vertexN, biomesData, seed);
            if (biom == default)
                return default;

            bool skip100R = biom.RequredObjects.Count == 0;
            bool skipHi = biom.HiiVerObjects.Count == 0;
            bool skipMid = biom.MiddVerObjects.Count == 0;
            bool skipLow = biom.LowVerObjects.Count == 0;

            Vector3 place100V = Vector3.zero;
            Vector3 placeHighV = Vector3.zero;
            Vector3 placeMidV = Vector3.zero;
            Vector3 placeLowV = Vector3.zero;
            

            if (!skip100R)
            {

                place100V = VectorUtils.GetRandomDir(1.5f, vertex);
                var obj100 = biom.RequredObjects[Random.Range(0, biom.RequredObjects.Count)];
                listObj.Add((obj100, place100V));
            }

            if (!skipHi)
            {
                if (Random.Range(0, 101) <= 10)
                {

                    do placeHighV = VectorUtils.GetRandomDir(Random.Range(0.5f, 2.5f), vertex);
                    while (placeHighV == place100V);

                    var objHi = biom.HiiVerObjects[Random.Range(0, biom.HiiVerObjects.Count)];
                    listObj.Add((objHi, placeHighV));

                }
            }

            if (!skipMid)
            {
                if (Random.Range(0, 101) <= 5)
                {

                    do placeMidV = VectorUtils.GetRandomDir(Random.Range(0.5f, 2.5f), vertex);
                    while (placeMidV == place100V || placeMidV == placeHighV);

                    var objMid = biom.MiddVerObjects[Random.Range(0, biom.MiddVerObjects.Count)];

                    listObj.Add((objMid, placeMidV));

                }
            }

            if (!skipLow)
            {
                if (Random.Range(0, 101) <= 1)
                {

                    do placeLowV = VectorUtils.GetRandomDir(Random.Range(0.5f, 2.5f), vertex);
                    while (placeLowV == place100V || placeLowV == placeHighV || placeLowV == placeMidV);

                    var objL = biom.LowVerObjects[Random.Range(0, biom.LowVerObjects.Count)];
                    listObj.Add((objL, placeLowV));

                }
            }

            return listObj;
        }
    }
}
