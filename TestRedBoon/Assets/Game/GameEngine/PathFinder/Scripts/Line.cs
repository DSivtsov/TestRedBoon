using System.Collections.Generic;
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

        public Line(Vector2 dotA, Vector2 dotB)
        {
            _factorX = -(dotB.y - dotA.y) / (dotB.x - dotA.x);
            _factorB = dotA.y + _factorX * dotA.x;
            if (Single.IsNaN(dotB.x - dotA.x) || (int)(dotB.x - dotA.x) == 0)
            {
                throw new NotFiniteNumberException($"(dotB.x - dotA.x)[{(int)(dotB.x - dotA.x)}] _factorX[{_factorX}]= -(dotB.y - dotA.y) / (dotB.x - dotA.x)");
            }
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
            float x, y;
            (float constValue, float minValue, float maxValue, EdgeType type) = StoreInfoEdges.GetEdgeInfo(currentTestingNumEdge);
            switch (type)
            {
                case EdgeType.Horizontal:
                    y = constValue;
                    if (StoreInfoEdges.InRange(FindXForY(y), minValue, maxValue))
                        return true;
                    return false;
                case EdgeType.Vertical:
                    x = constValue;
                    if (StoreInfoEdges.InRange(FindYForX(x), minValue, maxValue))
                        return true;
                    return false;
                default:
                    throw new NotSupportedException($"Wrong [{type}] Edge type");
            }
        }

        //private static bool InRange(float value, float minValue, float maxValue)
        //{
        //    return (int)(value - minValue) >= 0 && 0 <= (int)(maxValue - value);
        //}

        public override string ToString()
        {
            return $"_factorX[{_factorX}] _factorB[{_factorB}]";
        }

        internal bool IsBetweenLines(Line linePassed, Line notPassedLine)
        {
            StoreInfoEdges.GetMinMax(linePassed._factorX, notPassedLine._factorX, out float minFactorx, out float maxFactorx);
            return _factorX > minFactorx && _factorX < maxFactorx;
        }

    }
}