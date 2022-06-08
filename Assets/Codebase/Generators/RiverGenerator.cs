using Assets.Codebase.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Codebase.Generators
{
    public class RiverGenerator
    {
        
        readonly Vector2 firstPointHeightDelta;
        readonly AnimationCurve curveHeightByRiver;
        readonly AnimationCurve curveSideHeightByRiver;
        readonly AnimationCurve curveWidthByRiver;
        readonly int scaleStepAnalises;
        readonly int countRiver;
        readonly int riverHeight;
        readonly float riverWidth;
        readonly float gridSize;
        readonly float sandLvlHeight;
        readonly float terrainAmplitudeHeight;
        readonly float gridScale;

        DirectionType direction = DirectionType.None;
        HashSet<Vector3> baseVerticies;
        TerrainMap dataMap;

        public RiverGenerator(TerrainMap DataMap, Vector2 FirstPointHeightDelta, AnimationCurve CurveHeightByRiver, AnimationCurve CurveSideHeightByRiver, AnimationCurve CurveWidthByRiver, 
                              float GridScale, float GridSize, int ScaleStepAnalises, int CountRiver, int RiverHeight, int RiverWidth, float SandStartLvlHeight, float TerrainAmplitudeHeight)
        {
            dataMap = DataMap;
            firstPointHeightDelta = FirstPointHeightDelta;
            curveHeightByRiver = CurveHeightByRiver;
            curveSideHeightByRiver = CurveSideHeightByRiver;
            curveWidthByRiver = CurveWidthByRiver;
            scaleStepAnalises = ScaleStepAnalises;
            countRiver = CountRiver;
            riverHeight = RiverHeight;
            riverWidth = RiverWidth;
            sandLvlHeight = SandStartLvlHeight;
            terrainAmplitudeHeight = TerrainAmplitudeHeight;
            gridScale = GridScale;
            gridSize = GridSize;
        }

        /// <summary>
        /// Генерирует реки
        /// </summary>
        /// <returns>Возвращает мешмап с измененными координатами текущего меша</returns>
        public TerrainMap Generate()
        {
            List<Vector3> riversVerticies = new();
            baseVerticies = dataMap.Verticies.ToHashSet();


            for (int i = 0; i < countRiver; i++)
            {
                float deltaHeightMax = 0;
                float deltaHeightMin = 0;
                float getOffFromSide = 0;

                if (i == 0)
                {
                    deltaHeightMax = firstPointHeightDelta.y * dataMap.DeltaMaxHeight;
                    deltaHeightMin = firstPointHeightDelta.x * dataMap.DeltaMaxHeight;
                    getOffFromSide = Mathf.RoundToInt((float)(gridSize - gridSize * 0.9)) * gridScale; //смотрим от центра
                }
                else
                {
                    deltaHeightMax = firstPointHeightDelta.y * dataMap.DeltaMaxHeight;
                    deltaHeightMin = firstPointHeightDelta.x * dataMap.DeltaMaxHeight;
                    getOffFromSide = Mathf.Round(gridSize + gridSize * (i-countRiver)/countRiver) * gridScale; //идем от центра к краю

                }
                Debug.Log("getOffFromSide for step " + i +" = " + getOffFromSide);


                Vector3 firstVertex = baseVerticies.FirstOrDefault(v => v.x > getOffFromSide && v.z > getOffFromSide && v.y >= deltaHeightMin && v.y <= deltaHeightMax); //сделать тэйк
                if (firstVertex == default)//не нашлось подходящей высоты
                    continue;

                riversVerticies.Add(firstVertex);


                Vector3 foundVertex = GetMinAround(firstVertex);
                if (foundVertex == Vector3.zero) //нашел первый вектор
                    continue;

                //строим реку
                while (foundVertex.y > sandLvlHeight || //ниже уровня песка
                       riversVerticies.FirstOrDefault(rv => rv.x == foundVertex.x && rv.z == foundVertex.z) == default) //нет пересечения с рекой
                {


                    UpdateHeights(foundVertex, ref riversVerticies);

                    var potential_lakeCenter = foundVertex;

                    foundVertex = GetMinAround(foundVertex);
                    if (foundVertex == Vector3.zero)
                    {
                        
                        //делаем озеро.
                        var listLakeVert = CreateLake(potential_lakeCenter);

                        if (listLakeVert.Count == 0)
                            break;

                        var lakeHFixed = UpdateHeightLakeVertex(listLakeVert, potential_lakeCenter);
                        if (lakeHFixed.Item1.Count == 0)
                            break;

                        riversVerticies.AddRange(lakeHFixed.Item1);
                        //foundVertex = lakeHFixed.Item2;
                        Debug.Log("CreateLake");
                        break;
                    }
                }

            }

            if (riversVerticies.Count > 2) //больше одного вектора
            {
                Debug.Log("Update river vertx " + riversVerticies.Count);
                UpdateMeshArray(riversVerticies);
                Debug.Log("End river gen");
            }

            return dataMap;
        }

        /// <summary>
        /// Поиск нижней высоты вокруг точки
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vector3 GetMinAround(Vector3 point)
        {
            Vector3 foundPoint = default;
            float step = gridScale;
            var prev_direction = direction;

            //Обходим все 8 вершин вокруг точки
            for (int i = 0; i < 8; i++)
            {
                if (foundPoint != default)
                    break;

                switch (i)
                {
                    case 0:
                        if (prev_direction == DirectionType.xD_zUp || point.z - step < 0)
                            continue;

                        direction = DirectionType.xUp_zD;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x + step
                                                                    && v.z == point.z - step
                                                                    && v.y <= point.y);
                        break;
                    case 1:
                        if (prev_direction == DirectionType.xD_z)
                            continue;

                        direction = DirectionType.xUp_z;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x + step
                                                                    && v.z == point.z
                                                                    && v.y <= point.y);
                        break;
                    case 2:
                        if (prev_direction == DirectionType.xD_zD)
                            continue;

                        direction = DirectionType.xUp_zUp;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x + step
                                                                    && v.z == point.z + step
                                                                    && v.y <= point.y);
                        break;
                    case 3:
                        if (prev_direction == DirectionType.x_zUp || point.z - step < 0)
                            continue;

                        direction = DirectionType.x_zD;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x
                                                                    && v.z == point.z - step
                                                                    && v.y <= point.y);
                        break;
                    case 5:
                        if (prev_direction == DirectionType.x_zD)
                            continue;

                        direction = DirectionType.x_zUp;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x
                                                                    && v.z == point.z + step
                                                                    && v.y <= point.y);
                        break;
                    case 8:
                        if (prev_direction == DirectionType.xUp_zD || point.x - step < 0)
                            continue;

                        direction = DirectionType.xD_zUp;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x - step
                                                                    && v.z == point.z + step
                                                                    && v.y <= point.y);
                        break;
                    case 7:
                        if (prev_direction == DirectionType.xUp_z || point.x - step < 0)
                            continue;
                        direction = DirectionType.xD_z;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x - step
                                                                    && v.z == point.z
                                                                    && v.y <= point.y);
                        break;
                    case 6:
                        if (prev_direction == DirectionType.xUp_zUp || point.x - step < 0 && point.z - step < 0)
                            continue;

                        direction = DirectionType.xD_zD;
                        foundPoint = baseVerticies.FirstOrDefault(v => v.x == point.x - step
                                                                    && v.z == point.z - step
                                                                    && v.y <= point.y);
                        break;
                }
            }


            return foundPoint;
        }
 
        private void UpdateHeights(Vector3 foundVertex, ref List<Vector3> riversVerticies)
        {
            var tupleCenter =  UpdateHeightVertexCenter(foundVertex);
            foundVertex = tupleCenter.Item1;
            riversVerticies.Add(foundVertex);

            List<Vector3> listVertex = UpdateHeightSideVertex(foundVertex, tupleCenter.Item2);
            riversVerticies.AddRange(listVertex);

        }

        private (Vector3,float) UpdateHeightVertexCenter(Vector3 foundVertex)
        {
            float beforeY = foundVertex.y;
            float downCoefiic;
            if (foundVertex.y < 0)
                downCoefiic = curveHeightByRiver.Evaluate((-foundVertex.y) / dataMap.DeltaMinHeight); //учесть коэфицент высоты террейна
            else
                downCoefiic = curveHeightByRiver.Evaluate(foundVertex.y / dataMap.DeltaMaxHeight);

            float newHeight = foundVertex.y - (riverHeight * downCoefiic); //вычитаем значение высоты с учетом кривой и заданной степенью углубления реки
            foundVertex.y = newHeight; //обновили высоту

            return (foundVertex, beforeY);
        }

        private List<Vector3> UpdateHeightSideVertex(Vector3 point, float beforeY)
        {
            List<Vector3> verticies = new List<Vector3>();
            List<Vector3> tempVerticies = new List<Vector3>();
            float step = gridScale;
            float heightDownRiverCenter = Mathf.Abs(beforeY - point.y);

            

            float downWidthCoefiic;
            if (point.y < 0 )
                downWidthCoefiic = curveWidthByRiver.Evaluate((-point.y) / dataMap.DeltaMinHeight);
            else
                downWidthCoefiic = curveWidthByRiver.Evaluate(point.y / dataMap.DeltaMaxHeight);

            int width = Mathf.RoundToInt(riverWidth * downWidthCoefiic);


            //ширина
            for (int w = 1,iw = 0; w <= width; w++, iw++)
            {
         
                step = gridScale * w;
                var diagStep = step - gridScale;
                switch (direction)
                {
                    case DirectionType.xUp_zD: //left_up
                        {
                            tempVerticies = baseVerticies.Where(v => (v.x == point.x + step
                                                              && v.z == point.z + step)
                                                                        ||
                                                                (v.x == point.x - diagStep
                                                              && v.z == point.z - step)
                                                                        ||
                                                                (v.x == point.x + step
                                                              && v.z == point.z + diagStep)
                                                                        ||
                                                                (v.x == point.x - step
                                                              && v.z == point.z - step)
                                                                    )
                                                             .ToList();

                            break;
                        }
                    case DirectionType.xD_zUp: //right_down
                        {
                            tempVerticies = baseVerticies.Where(v => (v.x == point.x + step
                                                                  && v.z == point.z + step)
                                                                             ||
                                                                (v.x == point.x + diagStep
                                                              && v.z == point.z + step)
                                                                             ||
                                                              (v.x == point.x - step
                                                              && v.z == point.z - diagStep) 
                                                                             ||
                                                                (v.x == point.x - step
                                                              && v.z == point.z - step)
                                                                )
                                                         .ToList();
                            break;
                        }
                    case DirectionType.xUp_zUp: //right_up
                        {
                            tempVerticies = baseVerticies.Where(v => (v.x == point.x + step
                                                              && v.z == point.z - step)
                                                                          ||
                                                                (v.x == point.x + step
                                                              && v.z == point.z - diagStep)
                                                                          ||
                                                                (v.x == point.x - diagStep
                                                              && v.z == point.z + step)
                                                                          ||
                                                                 (v.x == point.x - step
                                                              && v.z == point.z + step)
                                                                    )
                                                             .ToList();

                            break;
                        }
                    case DirectionType.xD_zD: //left_down
                        {
                            tempVerticies = baseVerticies.Where(v => (v.x == point.x + step
                                                              && v.z == point.z - step)
                                                                          ||
                                                                (v.x == point.x + diagStep
                                                              && v.z == point.z - step )
                                                                          ||
                                                                (v.x == point.x - step 
                                                              && v.z == point.z + diagStep)
                                                                          ||
                                                                 (v.x == point.x - step
                                                              && v.z == point.z + step)
                                                                )
                                                         .ToList();
                            break;
                        }
                    case DirectionType.xUp_z:
                    case DirectionType.xD_z:
                        {
                            tempVerticies = baseVerticies.Where(v => (v.x == point.x
                                                                  && v.z == point.z - step)
                                                                             ||
                                                                     (v.x == point.x
                                                                  && v.z == point.z + step)
                                                                )
                                                         .ToList();

                            break;
                        }
                    case DirectionType.x_zD:
                    case DirectionType.x_zUp:
                        {
                            tempVerticies = baseVerticies.Where(v => (v.x == point.x + step
                                                                  && v.z == point.z)
                                                                             ||
                                                                     (v.x == point.x - step
                                                                  && v.z == point.z)
                                                                )
                                                         .ToList();
                            break;
                        }
                }


                var sub = width - iw;

                for (int i = 0; i < tempVerticies.Count; i++)
                {
                    float newHeight = point.y;
                    Vector3 side = tempVerticies[i];
                    newHeight += heightDownRiverCenter / sub;
                    side.y = newHeight;
                    tempVerticies[i] = side;
                }

                if (tempVerticies.Count > 0)
                    verticies.AddRange(tempVerticies);
            }

            //Увеличить стороны
            

            return verticies;
        }

        private (List<Vector3>,Vector3 ) UpdateHeightLakeVertex(List<Vector3> lakeVert, Vector3 centerPoint)
        {
            Vector3 min = default;
            for (int i = 0; i < lakeVert.Count; i++)
            {
                Vector3 side = lakeVert[i];
                side.y = centerPoint.y - Mathf.Abs(centerPoint.y * 0.1f);

                if (i == 0)
                    min = side;
                else if (min.y < side.y)
                    min = side;
                    
                    
                lakeVert[i] = side;
            }
            lakeVert.Remove(min);

            return (lakeVert,min);
        }


        private List<Vector3> CreateLake(Vector3 centerLake)
        {
            List<Vector3> lakeVerticies = new();
            Vector3 pointLake = Vector3.zero;
            float step = gridScale;
            bool find = true;
            float centerHeightMax = centerLake.y + 15;

            for (int scale = 1; scale <= 6; scale++) //степень расширения
            {
                if (!find)
                    break;

                find = false;
                var step_f = step * scale;
                for (int i = 0; i < 16; i++)
                {
                    if (pointLake != default)
                    {
                        lakeVerticies.Add(pointLake);
                        find = true;
                    }
                    pointLake = default;

                    switch (i)
                    {
                        case 0: // left_Up
                            //if (centerLake.z - step_f < 0)
                            //    continue;
                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step_f
                                                                       && v.z == centerLake.z - step_f);
                                                                       //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                                case 1:
                                    if (scale == 1)
                                        continue;

                            for (int j = 1; j <= scale - 1; j++)
                            {
                                lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step
                                                                           && v.z == centerLake.z - step * j));
                            }
                                
                                                                         //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                                break;
                        case 2: //up
                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step_f
                                                                       && v.z == centerLake.z);
                                                                     //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                        
                                case 3:
                                    if (scale == 1)
                                        continue;

                            for (int j = 1; j <= scale - 1; j++)
                                lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step_f
                                                                           && v.z == centerLake.z + step * j));
                                                                                   //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                                break;
                        case 4: //right_Up
                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step_f
                                                                       && v.z == centerLake.z + step_f);
                                                                       //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                        break;
                                case 5:
                                    if (scale == 1)
                                        continue;

                            for (int j = 1; j <= scale - 1; j++)
                                lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step * j
                                                                           && v.z == centerLake.z + step_f));
                                                                                   //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                                break;
                        case 6: //right

                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x
                                                                       && v.z == centerLake.z + step_f);
                                                                       //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                        break;
                                case 7:
                                    if (scale == 1)
                                        continue;

                                    for (int j = 1; j <= scale - 1; j++)
                                         lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x - step * j
                                                                                   && v.z == centerLake.z + step_f));
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                        case 8: //right_Down
                            //if (centerLake.x - step_f < 0)
                            //    continue;

                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x - step_f
                                                                       && v.z == centerLake.z + step_f);
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                                case 9:
                                    if (scale == 1)
                                        continue;

                                    for (int j = 1; j <= scale - 1; j++)
                                        lakeVerticies.Add(pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x - step_f
                                                                                   && v.z == centerLake.z + step * j));
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                        case 10: //down
                            //if (centerLake.x - step_f < 0)
                            //    continue;

                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x - step_f
                                                                       && v.z == centerLake.z);
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                                case 11:
                                    if (scale == 1)
                                        continue;

                                    for (int j = 1; j <= scale - 1; j++)
                                        lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x - step_f 
                                                                                   && v.z == centerLake.z - step * j));
                            // && (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                        case 12: //left_Down
                            //if (centerLake.x - step_f < 0 && centerLake.z - step_f < 0)
                            //    continue;

                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x - step_f
                                                                       && v.z == centerLake.z - step_f);
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                                case 13:
                                    if (scale == 1)
                                        continue;

                                    for (int j = 1; j <= scale - 1; j++)
                                        lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step * j
                                                                                   && v.z == centerLake.z - step_f));
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;

                        case 14: //left
                            //if (centerLake.z - step < 0)
                            //    continue;

                            pointLake = baseVerticies.FirstOrDefault(v => v.x == centerLake.x
                                                                       && v.z == centerLake.z - step_f);
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;
                                case 15:
                                    if (scale == 1)
                                        continue;

                                    for (int j = 1; j < scale - 1; j++)
                                        lakeVerticies.Add(baseVerticies.FirstOrDefault(v => v.x == centerLake.x + step * j
                                                                                         && v.z == centerLake.z - step_f));
                            //&& (v.y <= centerLake.y || v.y <= centerHeightMax));
                            break;

                    }
                }
            }
            //Обходим все 16 вершин вокруг точки
           


            return lakeVerticies;
        }

        private void UpdateMeshArray(List<Vector3> riversVerticies)
        {
            foreach (var foundVertex in riversVerticies)
            {
                var meshVertex = dataMap.Verticies.First(v => v.x == foundVertex.x && v.z == foundVertex.z);
                int index = Array.IndexOf(dataMap.Verticies, meshVertex);
                dataMap.Verticies[index].y = foundVertex.y;

                var firstInd = dataMap.Triangles.FindIndex(t => t.I1 == index);
                if (firstInd == -1)
                    continue;

                var firstTris = dataMap.Triangles.ElementAt(firstInd);

                var secondInd = dataMap.Triangles.FindIndex(t => t.I1 == firstTris.I3 && t.I3 == firstTris.I3 + 1);
                if (secondInd == -1)
                    continue;

                var secondTris = dataMap.Triangles.ElementAt(secondInd);

                //Lj,fdbnm 
            }
            
        }

       
    }
}
