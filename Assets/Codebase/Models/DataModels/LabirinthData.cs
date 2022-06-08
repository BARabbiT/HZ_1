using Assets.Codebase.Interfaces;
using Assets.Codebase.Models.Enums;
using Assets.Codebase.Models.WorldModels;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codebase.Models.DataModels
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Labirinth", order = 2)]
    public class LabirinthData : ScriptableObject
    {
        [Label("Тип")]
        public LabirinthType Type;

        [Label("Шум")]
        public NoiseData NoiseFilter;

        [Label("Материал")]
        public Material Material;

    }
}
