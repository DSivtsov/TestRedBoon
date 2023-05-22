using System;
using UnityEngine;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public enum EdgeType
    {
        Horizontal = 0,
        Vertical = 1,
    }

    public class Line
    {
        //factorY = 1
        // factorX * X +  1 * Y = factorB
        private float _factorX;   //k factor || Tang(alfa)
        private float _factorB;
        //private static Edge[] _arrEdges;
        private static (float constValue, float minValue, float maxValue, EdgeType type)[] _arrTupleEdges;
        private static bool _classIsInited = false;

        public static void InitLineClass(Edge[] arrEdges)
        {
            //_arrEdges = arrEdges;
            _arrTupleEdges = new (float constValue, float minValue, float maxValue, EdgeType type)[arrEdges.Length];
            for (int i = 0; i < arrEdges.Length; i++)
            {
                Edge currentEdge = arrEdges[i];
                _arrTupleEdges[i] = GetTupleEdge(currentEdge);
            }
            _classIsInited = true;
        }

        
        private static (float constValue, float minValue, float maxValue, EdgeType type) GetTupleEdge(Edge currentEdge)
        {
            if (currentEdge.Start.y == currentEdge.End.y)
            {
                GetMinMax(currentEdge.Start.x, currentEdge.End.x, out float minValue, out float maxValue);
                return (currentEdge.Start.y, minValue, maxValue, EdgeType.Horizontal);
            }
            else
            {
                GetMinMax(currentEdge.Start.y, currentEdge.End.y, out float minValue, out float maxValue);
                return (currentEdge.Start.x, minValue, maxValue, EdgeType.Vertical);
            }
        }

        private static void GetMinMax(float value1, float value2, out float minValue, out float maxValue)
        {
            minValue = Math.Min(value1, value2);
            maxValue = Math.Max(value1, value2);
        }

        public Line(Vector2 dotA, Vector2 dotB)
        {
            _factorX = - (dotB.y - dotA.y) / (dotB.x - dotA.x);
            _factorB = dotA.y + _factorX * dotA.x;
        }

        /// <summary>
        /// Get from Line the  factors for linear system equation, factorY always = 1
        /// </summary>
        /// <returns>factors for {i} equation</returns>
        public (float ai1, float ai2, float bi) GetDataForMatrix2x2() => (_factorX, 1f, _factorB);

        public float FindXForY(float y) => (_factorB - y) / _factorX;

        public float FindYForX(float x) => (_factorB - _factorX * x);

        //Currently all edges is Vertical or is Horizontal in this case the detection crossing Line with them can be simplify by use special structure
        /// <summary>
        /// Try Intersec Line With Edge
        /// </summary>
        /// <param name="currentTestingNumEdge"></param>
        /// <returns>true if Line interesect the edge</returns>
        internal bool TryIntersecLineWithEdge(int currentTestingNumEdge)
        {

            if (_classIsInited)
            {
                float x, y;
                var currentEdge = _arrTupleEdges[currentTestingNumEdge];
                float minValue = currentEdge.minValue;
                float maxValue = currentEdge.maxValue;
                switch (currentEdge.type)
                {
                    case EdgeType.Horizontal:
                        y = currentEdge.constValue;
                        if (InRange(FindXForY(y), minValue, maxValue))
                            return true;
                        return false;
                    case EdgeType.Vertical:
                        x = currentEdge.constValue;
                        if (InRange(FindYForX(x), minValue,maxValue))
                            return true;
                        return false;
                    default:
                        throw new NotSupportedException($"Wrong [{currentEdge.type}] Edge type");
                }
            }
            else
                throw new NotSupportedException($"Class [{this}] is not inited");
        }

        private static bool InRange(float y, float minValue, float maxValue)
        {
            return (int)(y - minValue) >= 0 && 0 <= (int)(maxValue - y);
        }

        public override string ToString()
        {
            return $"_factorX[{_factorX}] _factorB[{_factorB}]";
        }

        //internal (float min, float max) GetMinMaxFactorX(Line notPassedLine)
        //{
        //    GetMinMax(_factorX, notPassedLine._factorX, out float minFactorx, out float maxFactorx);
        //    Debug.Log($"minFactorx={minFactorx} maxFactorx={maxFactorx}");
        //    return (minFactorx, maxFactorx);
        //}

        internal bool IsBetweenLines(Line linePassed, Line notPassedLine)
        {
            GetMinMax(linePassed._factorX, notPassedLine._factorX, out float minFactorx, out float maxFactorx);
            return _factorX > minFactorx && _factorX < maxFactorx;
        }
    }
}