using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityMeshSimplifier;

namespace Assets.Scripts.Helpers
{
    

    public class AutoGenerateLODs : MonoBehaviour
    {
        [SerializeField, Tooltip("The simplification options.")]
        private SimplificationOptions simplificationOptions = SimplificationOptions.Default;
        [SerializeField, Tooltip("If renderers should be automatically collected, otherwise they must be manually applied for each level.")]
        private bool autoCollectRenderers = false;
        [SerializeField, Tooltip("The LOD levels.")]
        private LODLevel[] levels = null;

        private GameObject _gO;

        private void Start()
        {
            _gO = this.gameObject;
            SetDefaultSettings();
            GenerateLODs();
        }

        private void Reset()
        {
            SetDefaultSettings();
        }

        private void SetDefaultSettings()
        {
            simplificationOptions = SimplificationOptions.Default;
            autoCollectRenderers = false;
            levels = new LODLevel[]
            {
            new LODLevel(0.5f, 1f)
            {
                CombineMeshes = false,
                CombineSubMeshes = false,
                SkinQuality = SkinQuality.Auto,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ReceiveShadows = true,
                SkinnedMotionVectors = true,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes,
            },
            new LODLevel(0.17f, 0.65f)
            {
                CombineMeshes = true,
                CombineSubMeshes = false,
                SkinQuality = SkinQuality.Auto,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ReceiveShadows = true,
                SkinnedMotionVectors = true,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Simple
            },
            new LODLevel(0.02f, 0.4225f)
            {
                CombineMeshes = true,
                CombineSubMeshes = true,
                SkinQuality = SkinQuality.Bone2,
                ShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                ReceiveShadows = false,
                SkinnedMotionVectors = false,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off,
                ReflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off
            }
            };
        }

        private void GenerateLODs()
        {
            Debug.Log("Lods generating");

            LODGenerator.GenerateLODs(_gO, levels, autoCollectRenderers, simplificationOptions);
            //await UniTask.RunOnThreadPool(act);
            Debug.Log("Lods generating finished");
       
        }
    }
}
