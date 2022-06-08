using Assets.Codebase.Models.Enums;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Codebase.Models.DataModels
{
    /// <summary>
    /// Saved biome settings
    /// </summary>
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Biome", order = 1)]
    public class BiomeData : ScriptableObject
    {
        [Label("Тип")]
        public BiomType Type;

        [Range(-1, 1)]
        [Label("Макс. значение")]
        public float Max;

        [Range(-1, 1)]
        [Label("Мин. значение")]
        public float Min;

        [Label("Тип родителя")]
        public NoiseData NoiseFilter;

        [Label("Материал")]
        public Material Material;

        [Label("Свойство шейдера высота")]
        public string ShaderHeightName;

        [Label("100% Объекты")]
        public List<GameObject> RequredObjects;

        [Label("70%-90% Объекты")]
        public List<GameObject> HiiVerObjects;

        [Label("40%-70% Объекты")]
        public List<GameObject> MiddVerObjects;

        [Label("0%-40% Объекты")]
        public List<GameObject> LowVerObjects;
 
    }
}
