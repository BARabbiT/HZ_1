using Assets.Codebase.Generators;
using Assets.Codebase.Interfaces;
using Assets.Codebase.Models.Enums;
using NaughtyAttributes;
using RcherNZ.AccidentalNoise;
using System;
using UnityEngine;

namespace Assets.Codebase.Models.DataModels
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Noise", order = 1)]
    public class NoiseData : ScriptableObject
    {

        [Label("Тип шума")]
        public NoiseType typeNoise = NoiseType.PerlinNoise;

        [Label("Ширина шума")]
        public int width;

        [Label("Высота шума")]
        [ShowIf("typeNoise", NoiseType.AccidentialNoiseContouring)]
        public int height;

        [Range(0.01f, 500f)]
        [Label("Приближение")]
        public float scale = 58.58f;

        [Range(0.1f, 2f)]
        [Label("Частота генерации")]
        public float frequencyNoise = 1f;

        [Range(1, 7)]
        [Label("Детализация")]
        public int octaves = 6;

        [Range(0.001f, 3f)]
        [Label("Сглаживание\\резкость")]
        public float lacunarity = 1.72f;

        [Range(1, 200)]
        [Label("Степень высоты")]
        public int amplitudeHieght;

        [Range(0.1f, 1f)]
        [Label("Частота высоты")]
        [ShowIf("typeNoise", NoiseType.PerlinNoise)]
        public float persistenceAmplitude = 0.5f;

        [Label("Базовый тип")]
        [HideIf("typeNoise", NoiseType.PerlinNoise)]
        public BasisType basisTypeAcc = BasisType.Simplex;

        [Label("Тип фрактала")]
        [HideIf("typeNoise", NoiseType.PerlinNoise)]
        public FractalType fractalTypeAcc = FractalType.HybridMulti;

        [Label("Тип интерполяции")]
        [HideIf("typeNoise", NoiseType.PerlinNoise)]
        public InterpolationType interpolationTypeAcc = InterpolationType.Linear;

        [Range(0.1f, 10f)]
        [Label("Мощность шума")]
        [HideIf("typeNoise", NoiseType.PerlinNoise)]
        public float gainAcc = 0.1f;

        [Label("Curve коррекция высоты")]
        public AnimationCurve heightCurve;

        /// <summary>
        /// Get class for curent noise generator
        /// </summary>
        /// <param name="seed">map seed</param>
        /// <returns></returns>
        public INoise GetGenerator(int seed)
        {
            

            if (typeNoise == NoiseType.PerlinNoise)
            {
                return new NoiseGenerator.PerlinNoise(seed, octaves, scale, lacunarity, heightCurve)
                {
                    _amplitudeBase = amplitudeHieght,
                    _persistenceBase = persistenceAmplitude,
                    _frequencyBase = frequencyNoise
                };
            }
            else if (typeNoise == NoiseType.AccidentialNoiseContouring)
            {
                return new NoiseGenerator.AccidentionalNoiseCountouring(scale, heightCurve, width, height,
                            new ImplicitFractal(fractalTypeAcc, basisTypeAcc, interpolationTypeAcc)
                            {
                                Frequency = frequencyNoise,
                                Gain = gainAcc,
                                Lacunarity = lacunarity,
                                Octaves = octaves,
                                Offset = 1,
                                Seed = seed
                            });
            }
            else
            {
                return new NoiseGenerator.AccidentionalNoise(scale, heightCurve, width, amplitudeHieght,
                            new ImplicitFractal(fractalTypeAcc, basisTypeAcc, interpolationTypeAcc)
                            {
                                Frequency = frequencyNoise,
                                Gain = gainAcc,
                                Lacunarity = lacunarity,
                                Octaves = octaves,
                                Offset = 1,
                                Seed = seed
                            });
            }

        }

    }

   
}
