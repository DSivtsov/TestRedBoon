﻿using System;
using System.Collections.Generic;
using UnityEngine;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public enum RecType
    {
        FirstRect = 0,
        SecondRect = 1,
    }
    public static class StoreInfoEdges
    {
        private static EdgeInfo[] _edgesInfo;
        private static Edge[] _arrEdges;
        private static Rectangle[] _arrRectangle;

        private readonly struct EdgeInfo
        {
            internal readonly float ConstValue;
            internal readonly float MinValue;
            internal readonly float MaxValue;
            internal readonly EdgeType TypeEdge;

            internal EdgeInfo(Vector2 Start, Vector2 End)
            {
                float minValue, maxValue;
                if (Start.y == End.y)
                {
                    GetMinMax(Start.x, End.x, out minValue, out maxValue);
                    ConstValue = Start.y;
                    TypeEdge = EdgeType.Horizontal;
                }
                else
                {
                    GetMinMax(Start.y, End.y, out minValue, out maxValue);
                    ConstValue = Start.x;
                    TypeEdge = EdgeType.Vertical;
                }
                MinValue = minValue;
                MaxValue = maxValue;
            }
        }

        private static bool _classIsInited = false;

        internal static int GetNumRect(int numEdge, RecType recType) => numEdge + (int)recType;

        internal static int GetNumRectWithEdgeForSolution(int numEdge, SolutionSide solutionSide)
            => StoreInfoEdges.GetNumRect(numEdge, (solutionSide == SolutionSide.Start) ? RecType.FirstRect : RecType.SecondRect );

        internal static int GetNumEdge(int numRect, SolutionSide solutionSide)
        {
            return numRect - (int)((solutionSide == SolutionSide.Start) ? RecType.FirstRect : RecType.SecondRect);
        }

        internal static void InitStoreEdges(Edge[] arrEdges)
        {
            _arrEdges = arrEdges;
            _arrRectangle = new Rectangle[arrEdges.Length + 1];
            _edgesInfo = new EdgeInfo[arrEdges.Length];
            _arrRectangle[0] = arrEdges[0].First;
            for (int i = 0; i < arrEdges.Length; i++)
            {
                _arrRectangle[i+1] = arrEdges[i].Second;
                Edge currentEdge = arrEdges[i];
                _edgesInfo[i] = new EdgeInfo(currentEdge.Start,currentEdge.End);
            }
            _classIsInited = true;
        }

        internal static (float constValue, float minValue, float maxValue, EdgeType type) GetEdgeInfo(int numEdge)
        {
            if (_classIsInited)
            {
                EdgeInfo edgeInfo = _edgesInfo[numEdge];
                return (edgeInfo.ConstValue, edgeInfo.MinValue, edgeInfo.MaxValue, edgeInfo.TypeEdge);
            }
            else
                throw new NotSupportedException($"Class [{typeof(StoreInfoEdges)}] is not inited");
        }

        /// <summary>
        /// Detect the position of DotCrossing related to BaseDotAndEdge
        /// </summary>
        /// <param name="dotCrossing"></param>
        /// <param name="baseDotStart"></param>
        /// <param name="numEdge"></param>
        /// <returns>true if the DotCrossing Between BaseDot And Edge</returns>
        internal static bool IsDotCrossingBetweenBaseDotAndEdge(Vector2 dotCrossing, Vector2 baseDotStart, int numEdge)
        {
            (float constValueEdge, _, _, EdgeType type)  = GetEdgeInfo(numEdge);
            switch (type)
            {
                case EdgeType.Horizontal:
                    return Math.Sign(dotCrossing.y - baseDotStart.y) == Math.Sign(constValueEdge - dotCrossing.y);
                case EdgeType.Vertical:
                    return Math.Sign(dotCrossing.x - baseDotStart.x) == Math.Sign(constValueEdge - dotCrossing.x);
                default:
                    throw new NotSupportedException($"Wrong Value EdgeType[{type}]");
            }
        }

        internal static (bool IsDotInRect, int numRect) IsDotInRectBetweenRecBaseDotAndRectEdge(Vector2 dotCrossing, int numRectBaseDot, int idxLastCrossingEdge,
            SolutionSide solutionSide)
        {
            int numRectEdgeEnd = GetNumRect(idxLastCrossingEdge, (solutionSide == SolutionSide.Start) ? RecType.FirstRect : RecType.SecondRect);
            return DotInRectangles(dotCrossing, numRectBaseDot, numRectEdgeEnd);
        }

        internal static (bool dotInRec, int numRect) IsDotInRectBetweenEdges(Vector2 dotCrossing, int numLastCrossingEdgeFromStartSolution, int numLastCrossingEdgeFromEndSolution)
        {
            int numRectEdgeStart = GetNumRect(numLastCrossingEdgeFromStartSolution, RecType.SecondRect);
            int numRectEdgeEnd = GetNumRect(numLastCrossingEdgeFromEndSolution, RecType.FirstRect);
            return DotInRectangles(dotCrossing, numRectEdgeStart, numRectEdgeEnd);
        }

        private static (bool dotInRec, int numRect) DotInRectangles(Vector2 dotCrossing, int numRectEdgeStart, int numRectEdgeEnd)
        {
            if (_classIsInited)
            {
                for (int numRect = numRectEdgeStart; numRect <= numRectEdgeEnd; numRect++)
                {
                    if (IsDotInRect(dotCrossing, _arrRectangle[numRect]))
                        return (true, numRect);
                }
                return (false, -1);
            }
            else
                throw new NotSupportedException($"Class [{typeof(StoreInfoEdges)}] is not inited");
        }


        internal static void GetMinMax(float value1, float value2, out float minValue, out float maxValue)
        {
            minValue = Math.Min(value1, value2);
            maxValue = Math.Max(value1, value2);
        }

        /// <summary>
        /// Dot into Rect or on its edges
        /// </summary>
        /// <param name="dot"></param>
        /// <param name="checkedRec"></param>
        /// <returns></returns>
        internal static bool IsDotInRect(Vector2 dot, Rectangle checkedRec)
        {
            return InRange(dot.x, checkedRec.Min.x, checkedRec.Max.x) && InRange(dot.y, checkedRec.Min.y, checkedRec.Max.y);
        }

        internal static bool InRange(float value, float minValue, float maxValue)
        {
            return (int)(value - minValue) >= 0 && (int)(maxValue - value) >= 0 ;
        }

        internal static IEnumerable<Vector2> GitListDotsEdge(int numEdge)
        {
            Edge edge = _arrEdges[numEdge];
            yield return edge.Start;
            yield return edge.End;
        }
    }
}