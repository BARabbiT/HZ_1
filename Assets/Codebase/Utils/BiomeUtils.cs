using Assets.Codebase.Models.DataModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Codebase.Utils
{
    public static class BiomeUtils
    {
        public static BiomeData GetBiome(float x, float z, float y, List<BiomeData> biomeDatas, int seed)
        {
            var biomeResult = biomeDatas.FirstOrDefault(b => b.NoiseFilter == null && y >= b.Min && y <= b.Max);
            if (biomeResult != default)
                return biomeResult;

            //запрос по картам высот. 
            foreach (var biome in biomeDatas.Where(b=>b.NoiseFilter != null))
            {
                var gen = biome.NoiseFilter.GetGenerator(seed);
                gen.GetHeight(x, z, out float alphaH, out bool isBound);

                if (alphaH >= biome.Min && alphaH <= biome.Max)
                    return biome;
            }

            return default;
        }

        public static BiomeData GetBiome(float y, List<BiomeData> biomeDatas)
        {
            var biomeResult = biomeDatas.FirstOrDefault(b => b.NoiseFilter == null && y >= b.Min && y <= b.Max);
            if (biomeResult != default)
                return biomeResult;

            return default;
        }

        public static BiomeData GetBiome(Vector3 vector, List<BiomeData> biomeDatas, int seed)
        {
            float x = vector.x;
            float z = vector.z;
            float y = vector.y;


            var biomeResult = biomeDatas.FirstOrDefault(b => b.NoiseFilter == null && y >= b.Min && y <= b.Max);
            if (biomeResult != default)
                return biomeResult;

            //запрос по картам высот. 
            foreach (var biome in biomeDatas.Where(b => b.NoiseFilter != null))
            {
                var gen = biome.NoiseFilter.GetGenerator(seed);
                gen.GetHeight(x, z, out float alphaH, out bool isBound);

                if (alphaH >= biome.Min && alphaH <= biome.Max)
                    return biome;
            }

            return default;
        }

    }
}
