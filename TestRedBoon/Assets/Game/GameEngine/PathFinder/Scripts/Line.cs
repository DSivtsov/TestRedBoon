using System.Collections.Generic;
using System;
using UnityEngine;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public enum LineType
    {
        Horizontal = 0,
        Vertical = 1,
    }

    public class Line
    {
        private const float FactorYNormolizedLine = 1f;
        // factorX * X +  1 * Y = factorB
        protected readonly float _factorX;   //k factor || Tang(alfa)
        protected readonly float _factorY;
        protected readonly float _factorB;

        protected Line(float factorX, float factorY, float factorB)
        {
            _factorX = factorX;
            _factorB = factorB;
            _factorY = factorY;
            //_factorX = -(dotB.y - dotA.y) / (dotB.x - dotA.x);
            //_factorB = dotA.y + _factorX * dotA.x;
            //if (Single.IsNaN(dotB.x - dotA.x) || (int)(dotB.x - dotA.x) == 0)
            //{
            //    throw new NotFiniteNumberException($"(dotB.x - dotA.x)[{(int)(dotB.x - dotA.x)}] _factorX[{_factorX}]= -(dotB.y - dotA.y) / (dotB.x - dotA.x)");
            //}
        }

        internal static Line CreateLine(Vector2 dotA, Vector2 dotB)
        {
            float deltaX = dotB.x - dotA.x;
            float deltaY = dotB.y - dotA.y;
            float factorX, factorB;
            if ((int)deltaY == 0)
            {
                //factorX = 0;
                //factorY = 1f;
                factorB = dotA.y;
                return new LineHorizontal(factorB);
            }
            else
            {
                if ((int)deltaX == 0)
                {
                    //factorX = 1f;
                    //factorY = 0;
                    factorB = dotA.x;
                    return new LineVertical(factorB);
                }
                else
                {
                    factorX = -deltaY / deltaX;
                    //factorY = 1f;
                    factorB = dotA.y + factorX * dotA.x;
                    return new Line(factorX, FactorYNormolizedLine, factorB);
                }
            }
        }

        internal virtual (Vector2 startDot, Vector2 endDot) GetDotsforScreen(int widthHalfField, int heightHalfField)
        {
            Vector2 startDot = new Vector2(-widthHalfField, (float)FindYForX(-widthHalfField));
            Vector2 endDot = new Vector2(widthHalfField, (float)FindYForX(widthHalfField));
            return (startDot, endDot);
        }

        /// <summary>
        /// Get from Line the  factors for linear system equation, factorY always = 1
        /// </summary>
        /// <returns>factors for {i} equation</returns>
        internal virtual (float ai1, float ai2, float bi) GetDataForMatrix2x2() => (_factorX, _factorY, _factorB);

        private float FindXForY(float y) => (_factorB - y) / _factorX;

        private float FindYForX(float x) => (_factorB - _factorX * x);

        //Currently all edges is Vertical or is Horizontal in this case the detection crossing Line with them can be simplify by use special structure
        /// <summary>
        /// Try Intersec Line With Edge
        /// </summary>
        /// <param name="currentTestingNumEdge"></param>
        /// <returns>true if Line interesect the edge</returns>
        internal virtual bool TryIntersecLineWithEdge(int currentTestingNumEdge)
        {
            float x, y;
            (float constValue, float minValue, float maxValue, LineType type) = StoreInfoEdges.GetEdgeInfo(currentTestingNumEdge);
            switch (type)
            {
                case LineType.Horizontal:
                    y = constValue;
                    if (StoreInfoEdges.InRange(FindXForY(y), minValue, maxValue))
                        return true;
                    return false;
                case LineType.Vertical:
                    x = constValue;
                    if (StoreInfoEdges.InRange(FindYForX(x), minValue, maxValue))
                        return true;
                    return false;
                default:
                    throw new NotSupportedException($"Wrong [{type}] Edge type");
            }
        }

        public override string ToString()
        {
            return $"_factorX[{_factorX}] _factorY[{_factorY}] _factorB[{_factorB}]";
        }
    }
    public sealed class LineHorizontal : Line
    {// factorX * X +  factorY * Y = factorB
        private const float FactorXVerticalLine = 0;
        private const float FactorYVerticalLine = 1f;
        public LineHorizontal(float factorB) : base(FactorXVerticalLine, FactorYVerticalLine, factorB) { }

        internal override (Vector2 startDot, Vector2 endDot) GetDotsforScreen(int widthHalfField, int heightHalfField)
        {
            Vector2 startDot = new Vector2(-widthHalfField, _factorB);
            Vector2 endDot = new Vector2(widthHalfField, _factorB);
            return (startDot, endDot);
        }

        internal override bool TryIntersecLineWithEdge(int currentTestingNumEdge)
        {
            float y = _factorB;
            (float constValue, float minValue, float maxValue, LineType lineTypeEdge) = StoreInfoEdges.GetEdgeInfo(currentTestingNumEdge);
            switch (lineTypeEdge)
            {
                case LineType.Horizontal:
                    if ((int)(y - constValue) == 0)
                        return true;
                    return false;
                case LineType.Vertical:
                    if (StoreInfoEdges.InRange(y, minValue, maxValue))
                        return true;
                    return false;
                default:
                    throw new NotSupportedException($"Wrong [{lineTypeEdge}] Edge type");
            }
        }
    }

    public sealed class LineVertical : Line
    {// factorX * X +  factorY * Y = factorB
        private const float FactorXVerticalLine = 1f;
        private const float FactorYVerticalLine = 0;
        public LineVertical(float factorB) : base(FactorXVerticalLine, FactorYVerticalLine, factorB) { }

        internal override (Vector2 startDot, Vector2 endDot) GetDotsforScreen(int widthHalfField, int heightHalfField)
        {
            Vector2 startDot = new Vector2(_factorB, -heightHalfField);
            Vector2 endDot = new Vector2(_factorB, heightHalfField);
            return (startDot, endDot);
        }
        internal override bool TryIntersecLineWithEdge(int currentTestingNumEdge)
        {
            float x = _factorB;
            (float constValue, float minValue, float maxValue, LineType lineTypeEdge) = StoreInfoEdges.GetEdgeInfo(currentTestingNumEdge);
            switch (lineTypeEdge)
            {
                case LineType.Vertical:
                    if ((int)(x - constValue) == 0)
                        return true;
                    return false;
                case LineType.Horizontal:
                    if (StoreInfoEdges.InRange(x, minValue, maxValue))
                        return true;
                    return false;
                default:
                    throw new NotSupportedException($"Wrong [{lineTypeEdge}] Edge type");
            }
        }
    }
}