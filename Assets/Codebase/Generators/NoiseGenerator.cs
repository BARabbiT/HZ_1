///////////////////////////////////
/// author: Vohmyakov Andrey
///////////////////////////////////

using Assets.Codebase.Interfaces;
using System;
using UnityEngine;
using RcherNZ.AccidentalNoise;

namespace Assets.Codebase.Generators
{
    /// <summary>
    /// Noise generators base on other types of noises
    /// </summary>
    public static class NoiseGenerator
    {
        /// <summary>
        /// Perlin noise generator
        /// </summary>
        public class PerlinNoise : INoise
        {

            readonly int _octaves; 
            readonly float _scale; 
            readonly float _lacunarity;
            readonly AnimationCurve _heightCurve; 
            readonly Vector2[] _octaveOffsets;
            public float _amplitudeBase = 20;
            public float _persistenceBase = 0.5f;
            public float _frequencyBase = 1;

            public PerlinNoise(int seed, int octaves, float scale, float lacunarity, AnimationCurve heightCurve)
            {
                _octaves = octaves;
                _scale = scale == 0 ? 0.0001f : scale;
                _lacunarity = lacunarity;
                _heightCurve = heightCurve;
                _octaveOffsets = GetOctavesOffset(seed, octaves);
            }

            public float GetHeight(float x, float z)
            {
                float noiseHeight = 0;
                float amplitude = _amplitudeBase;
                float persistence = _persistenceBase;
                float frequency = _frequencyBase;

                // loop over octaves
                for (int y = 0; y < _octaves; y++)
                {
                    float mapZ = z / _scale * frequency + _octaveOffsets[y].y;
                    float mapX = x / _scale * frequency + _octaveOffsets[y].x;

                   
                    //The *2-1 is to create a flat floor level
                    float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;

                    if (perlinValue > 0.8 && perlinValue < 0.9)
                        perlinValue = UnityEngine.Random.Range(0.8f, 0.83f);

                    noiseHeight += _heightCurve.Evaluate(perlinValue) * amplitude;
                    frequency *= _lacunarity;
                    amplitude *= persistence;
                }
                return noiseHeight;
            }

            public float GetHeight(float x, float y, float z)
            {
                throw new NotImplementedException();
            }

            public float ConvertAlphaToHeight(float heightTerrainBeforeEval)
            {
                throw new NotImplementedException();
            }

            public float GetHeight(float x, float z, out float clearHeight, out bool isBorder)
            {
                throw new NotImplementedException();
            }

            private static Vector2[] GetOctavesOffset(int seed, int octaves)
            {
                // changes area of map
                System.Random prng = new System.Random(seed);
                Vector2[] octaveOffsets = new Vector2[octaves];

                for (int o = 0; o < octaves; o++)
                {
                    float offsetX = prng.Next(-100000, 100000);
                    float offsetY = prng.Next(-100000, 100000);
                    octaveOffsets[o] = new Vector2(offsetX, offsetY);
                }

                return octaveOffsets;
            }
        }

        /// <summary>
        /// Accidentinal lib base noise generator
        /// </summary>
        public class AccidentionalNoise : INoise
        {
            readonly ImplicitFractal _implicitFractalTerrain;
            readonly AnimationCurve _heightCurve; 
            readonly float _scale; 
            readonly int _gridSize;
            readonly float _amplitudeBase;
            readonly float _stepBoundDown;
            readonly float _stepBoundDownCoord;
            readonly float _maxSide;
            readonly float _maxSideCoord;

            public AccidentionalNoise(float scale, AnimationCurve heightCurve, int gridSize, float amplitudeBase, ImplicitFractal implicitFractal)
            {
                _scale = scale == 0 ? 0.0001f : scale;
                _heightCurve = heightCurve;
                _implicitFractalTerrain = implicitFractal;
                _gridSize = gridSize;
                _amplitudeBase = amplitudeBase;
                _stepBoundDown = 60;
                _maxSide = _gridSize - _stepBoundDown;

                _stepBoundDownCoord = _stepBoundDown / _gridSize * _scale + 10000f;
                _maxSideCoord = _maxSide / _gridSize * _scale + 10000f;

            }

            /// <summary>
            /// Get height by x,z coordinates of map, from current noise settings
            /// </summary>
            /// <param name="x">x coordinate of mesh</param>
            /// <param name="z"></param>
            /// <param name="clearHeight"></param>
            /// <param name="isBound"></param>
            /// <returns></returns>
            public float GetHeight(float x, float z, out float clearHeight, out bool isBound)
            {
                isBound = false;
                float xT = x / _gridSize * _scale + 10000f;
                float zT = z / _gridSize * _scale + 10000f;

                clearHeight = (float)_implicitFractalTerrain.Get(xT, zT);
                

                //Алгоритм формирования углов и краев карты
                if (x == 0 || x == _gridSize || z == 0 || z == _gridSize)
                {
                    clearHeight = -1;
                }
                else if ((x < _stepBoundDown || x > _maxSide) && (z < _stepBoundDown || z > _maxSide)) //угол карты
                {
                    isBound = true;
                    float maxCoordinByStepBound = _stepBoundDown / _gridSize * _scale + 10000f;
                    float maxCoordinByMaxSide = _maxSide / _gridSize * _scale + 10000f;

                    if (x < _stepBoundDown && z < _stepBoundDown) //0,0 corner
                    {
                        float maxSimmetryCornerHeight = (float)_implicitFractalTerrain.Get(maxCoordinByStepBound, maxCoordinByStepBound); //макс.высота линии симетрии по самой далней точке
                        if (x >= z) //правый квадрат,
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, x / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, z / x);

                            //можно попробовать вычислять высоту средним между реальным значением и полученным по степени влияния макс высоты в границах угла по линии симетрии
                        }
                        else if (x < z) //левый квадрат
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, z / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, x / z);
                        }
                        
                    }
                    else if (x < _stepBoundDown && z > _maxSide) //0,1 corner
                    {
                        float maxSimmetryCornerHeight = (float)_implicitFractalTerrain.Get(maxCoordinByStepBound, maxCoordinByMaxSide); //макс.высота линии симетрии по самой далней точке
                        float zCor = _gridSize - z;
                        if (x >= zCor) //правый квадрат,
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, x / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, zCor / x);
                        }
                        else if (x < zCor) //левый квадрат
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, zCor / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, x / zCor);
                        }
                    }
                    else if (x > _maxSide && z > _maxSide) //1,1 corner
                    {
                        float maxSimmetryCornerHeight = (float)_implicitFractalTerrain.Get(maxCoordinByMaxSide, maxCoordinByMaxSide); //макс.высота линии симетрии по самой далней точке
                        float zCor = _gridSize - z;
                        float xCor = _gridSize - x;
                        if (xCor >= zCor) //правый квадрат,
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, xCor / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, zCor / xCor);
                        }
                        else if (xCor < zCor) //левый квадрат
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, zCor / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, xCor / zCor);
                        }
                    }
                    else if (x > _maxSide && z < _stepBoundDown)
                    {
                        float maxSimmetryCornerHeight = (float)_implicitFractalTerrain.Get(maxCoordinByMaxSide, maxCoordinByStepBound); //макс.высота линии симетрии по самой далней точке
                        float xCor = _gridSize - x;
                        if (xCor >= z) //правый квадрат,
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, xCor / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, z / xCor);
                        }
                        else if (xCor < z) //левый квадрат
                        {
                            float maxHeightThisLine = Mathf.Lerp(-1, maxSimmetryCornerHeight, z / _stepBoundDown);
                            clearHeight = Mathf.Lerp(-1, maxHeightThisLine, xCor / z);
                        }
                    }
                }
                else if (x < _stepBoundDown || x > _maxSide)
                {
                    isBound = true;
                    clearHeight = CalcBoundByX(x, zT);
                }
                else if (z < _stepBoundDown || z > _maxSide)
                {
                    isBound = true;
                    clearHeight = CalcBoundByZ(z, xT);
                }

                float heightTerrain;
                if (!isBound)
                    heightTerrain = _heightCurve.Evaluate(clearHeight);
                else
                    heightTerrain = clearHeight;

                if (clearHeight <= -0.8) //sand
                    return heightTerrain * _amplitudeBase / 1.92f;
                else if (clearHeight >= 0) //rock
                    return heightTerrain * _amplitudeBase * (2 - heightTerrain);
                else
                    return heightTerrain * _amplitudeBase / 2;
 
            }

            private float CalcBoundByX(float x, float zT)
            {
                float iterator;
                float endBoundHeight;

                if (x < _stepBoundDown)
                {
                    iterator = x; //от меньшего к большему
                    endBoundHeight = (float)_implicitFractalTerrain.Get(_stepBoundDownCoord, zT);
                }
                else
                {
                    iterator = _gridSize - x; //от большего к меньшему
                    endBoundHeight = (float)_implicitFractalTerrain.Get(_maxSideCoord, zT);
                }
                return Mathf.Lerp(-1, endBoundHeight, iterator / _stepBoundDown);
            }

            private float CalcBoundByZ(float z, float xT)
            {
                float iterator;
                float endBoundHeight;

                if (z < _stepBoundDown)
                {
                    iterator = z; //от меньшего к большему
                    endBoundHeight = (float)_implicitFractalTerrain.Get(xT, _stepBoundDownCoord);
                }
                else
                {
                    iterator = _gridSize - z; //от большего к меньшему
                    endBoundHeight = (float)_implicitFractalTerrain.Get(xT, _maxSideCoord);
                }
                return Mathf.Lerp(-1, endBoundHeight, iterator / _stepBoundDown);
                
            }

            public float GetHeight(float x, float y, float z)
            {
                throw new NotImplementedException();
            }

            public float GetHeight(float x, float z)
            {
                float xT = x / _gridSize * _scale + 1000f;
                float zT = z / _gridSize * _scale + 1000f;

                float heightTerrainBeforeEval = (float)_implicitFractalTerrain.Get(xT, zT);
                float heightTerrain = _heightCurve.Evaluate(heightTerrainBeforeEval);

                //Исключаем curve для отрицательных координат и уменьшаем степень вмятия в пять раз. 
                if (heightTerrainBeforeEval <= -0.8) //sand
                    return heightTerrain * _amplitudeBase / 1.92f;
                else if (heightTerrainBeforeEval >= 0) //rock
                    return heightTerrain * _amplitudeBase * (2 - heightTerrain);
                else
                    return heightTerrain * _amplitudeBase / 2;
            }

            public float ConvertAlphaToHeight(float heightTerrainBeforeEval)
            {
                float heightTerrain = _heightCurve.Evaluate(heightTerrainBeforeEval);


                //Исключаем curve для отрицательных координат и уменьшаем степень вмятия в пять раз. 
                if (heightTerrainBeforeEval <= -0.8) //sand
                    return heightTerrainBeforeEval * _amplitudeBase / 1.92f;
                else if (heightTerrainBeforeEval >= 0) //rock
                    return heightTerrainBeforeEval * _amplitudeBase * (2 - heightTerrainBeforeEval);
                else
                    return heightTerrainBeforeEval * _amplitudeBase / 2;
            }
        }

        /// <summary>
        /// Accidentinal lib base noise generator for DualCountouring algorithm
        /// </summary>
        public class AccidentionalNoiseCountouring : INoise
        {
            readonly ImplicitFractal _implicitFractalTerrain;
            readonly AnimationCurve _heightCurve;
            readonly float _scale;
            readonly int _width;
            readonly int _height;

            public AccidentionalNoiseCountouring(float scale, AnimationCurve heightCurve, int width, int height, ImplicitFractal implicitFractal)
            {
                _scale = scale == 0 ? 0.0001f : scale;
                _heightCurve = heightCurve;
                _implicitFractalTerrain = implicitFractal;
                _width = width;
                _height = height;

            }

            public float GetHeight(float x, float y, float z)
            {

                float xT = x / _width * _scale;
                float zT = z / _width * _scale;
                float yT = y /_height * _scale;
 
                float heightTerrain = (float)_implicitFractalTerrain.Get(xT, yT, zT);
                //heightTerrain = heightCurve.Evaluate(heightTerrainBeforeEval);


                return heightTerrain;


            }

            public float GetHeight(float x, float z)
            {
                float xT = x / _width * _scale;
                float zT = z / _width * _scale;
                //float yT = y / height * scale;

                return (float)_implicitFractalTerrain.Get(xT, zT);
            }

            public float ConvertAlphaToHeight(float heightTerrainBeforeEval)
            {
                throw new NotImplementedException();
            }

            public float GetHeight(float x, float z, out float clearHeight, out bool isBorder)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Accidentinal lib based noise generator with river base on other noise 
        /// </summary>
        public class AccidentionalNoiseWithSecondNoiseRivers : INoise
        {
            readonly ImplicitFractal _implicitFractalTerrain;
            readonly ImplicitFractal _implicitFractalRiver;
            readonly AnimationCurve _heightCurve;
            readonly float _scale;
            readonly int _gridSize;
            readonly float _amplitudeBase;
            readonly float _amplitudeBaseRiver;
            readonly AnimationCurve _heightCurveRiver;
            readonly float _scaleRiver;
            readonly float _accuracyDetectRiver;


            public AccidentionalNoiseWithSecondNoiseRivers(float scale, AnimationCurve heightCurve, int gridSize, float amplitudeBase, ImplicitFractal implicitFractal,
                                      float accuracyDetectRiver, float scaleRiver, AnimationCurve heightCurveRiver, float amplitudeBaseRiver, ImplicitFractal implicitFractalRiver)
            {
                _scale = scale == 0 ? 0.0001f : scale;
                _heightCurve = heightCurve;
                _implicitFractalTerrain = implicitFractal;
                _gridSize = gridSize;
                _amplitudeBase = amplitudeBase;

                _scaleRiver = scaleRiver;
                _amplitudeBaseRiver = amplitudeBaseRiver;
                _heightCurveRiver = heightCurveRiver;
                _implicitFractalRiver = implicitFractalRiver;
                _accuracyDetectRiver = accuracyDetectRiver;
            }

            public float GetHeight(float x, float z)
            {
                float xR = x / _gridSize * _scaleRiver + 1000f;
                float zR = z / _gridSize * _scaleRiver + 1000f;

                float xT = x / _gridSize * _scale + 1000f;
                float zT = z / _gridSize * _scale + 1000f;



                float heightTerreinSubscratRiver = 0;
                float heightTerrain = 0;

                //Базовое значение реки и террейна
                float heightRiverBeforeEval = (float)_implicitFractalRiver.Get(xR, zR);
                float heightTerrainBeforeEval = (float)_implicitFractalTerrain.Get(xT, zT);

                //Если шум реки больше 0, значит, берем среднеее значение
                if (heightRiverBeforeEval > _accuracyDetectRiver)
                {
                    //Берем кривую сопоставления высоты террейна для гор, потмоу что по ней формируется правильная кривая горы
                    heightTerrain = _heightCurve.Evaluate(heightTerrainBeforeEval);
                    //Кривую сопоставления высоты реки по террейну, если гора например = 1, то вернуть 0, что бы никак не накладывать реку
                    //Если гора равно 0, то вернуть вычесть 1, поскольку это террейн
                    heightTerreinSubscratRiver = _heightCurveRiver.Evaluate(heightTerrainBeforeEval);
                    heightTerrain -= heightTerreinSubscratRiver / _amplitudeBaseRiver;


                    return heightTerrain * _amplitudeBase;
                }
                else
                    heightTerrain = _heightCurve.Evaluate(heightTerrainBeforeEval);


                //Исключаем curve для отрицательных координат и уменьшаем степень вмятия в пять раз. 
                if (heightTerrainBeforeEval < 0)
                    return heightTerrainBeforeEval * _amplitudeBase / 4;

                return heightTerrain * _amplitudeBase;


            }
            public float GetHeight(float x, float y, float z)
            {
                throw new NotImplementedException();
            }
            public float ConvertAlphaToHeight(float heightTerrainBeforeEval)
            {
                throw new NotImplementedException();
            }

            public float GetHeight(float x, float z, out float clearHeight, out bool isBorder)
            {
                throw new NotImplementedException();
            }
        }
    }
}
