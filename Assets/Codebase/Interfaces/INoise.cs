using UnityEngine;

namespace Assets.Codebase.Interfaces
{
    public interface INoise
    {
        float GetHeight(float x, float z);

        /// <summary>
        /// Get height by x,z coordinates of map, from current noise settings
        /// </summary>
        /// <param name="x">x coordinate of mesh</param>
        /// <param name="z">z coordinate of mesh</param>
        /// <param name="clearHeight">out value height without any changes</param>
        /// <param name="isBound">curent coordinate is bound of map</param>
        /// <returns></returns>
        float GetHeight(float x, float z, out float clearHeight, out bool isBorder);

        float GetHeight(float x, float y, float z);
        float ConvertAlphaToHeight(float heightTerrainBeforeEval);
    }
}