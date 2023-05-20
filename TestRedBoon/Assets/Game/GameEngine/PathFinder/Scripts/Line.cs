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
                float minValue = Math.Min(currentEdge.Start.x, currentEdge.End.x);
                float maxValue = Math.Max(currentEdge.Start.x, currentEdge.End.x);
                return (currentEdge.Start.y, minValue, maxValue, EdgeType.Horizontal); 
            }
            else
            {
                float minValue = Math.Min(currentEdge.Start.y, currentEdge.End.y);
                float maxValue = Math.Max(currentEdge.Start.y, currentEdge.End.y);
                return (currentEdge.Start.x, minValue, maxValue, EdgeType.Vertical);
            }
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

        //internal void LineCrossingEdge(Vector2 start, Vector2 end)
        //{
        //    //Demands get LineEdge from edge
        //    //Build Matrix2x2 with LineEdges and Line
        //    //Get Solution
        //    throw new NotImplementedException();
        //}

        //internal void TryIntersecLineWithEdge(int currentTestingNumEdge)
        //{
        //    Edge curentEdge = _arrEdges[currentTestingNumEdge];
        //    LineCrossingEdge(curentEdge.Start, curentEdge.End);
        //}

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
                var currentEdge = _arrTupleEdges[currentTestingNumEdge];
                switch (currentEdge.type)
                {
                    case EdgeType.Horizontal:
                        float x = (_factorB - currentEdge.constValue);
                        if (x >= currentEdge.minValue && x <= currentEdge.maxValue)
                            return true;
                        return false;
                    case EdgeType.Vertical:
                        float y = (_factorB - _factorX * currentEdge.constValue);
                        if (y >= currentEdge.minValue && y <= currentEdge.maxValue)
                            return true;
                        return false;
                    default:
                        throw new NotSupportedException($"Wrong [{currentEdge.type}] Edge type");
                } 
            }
            else
                throw new NotSupportedException($"Class [{this}] is not inited");
        }

    }
}