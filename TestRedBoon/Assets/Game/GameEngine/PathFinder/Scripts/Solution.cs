using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public interface ISolution
    {
        public IEnumerable<Solution> GetListSolution();
        public IEnumerable<Line> GetListLinesFromSolution();
        public IEnumerable<Vector2> GetListBasedDotsSolution();
        public int NumEdge { get; }
    }

    public class Solution // angleLineB > angleLineA in degrees // in k factor
    {
        public readonly Line LineB;
        public readonly Line LineA;
        public readonly Vector2 BaseDot;
        public readonly DotIntersec DotCrossing;

        public Solution(List<Line> lines, Vector2 baseDot, DotIntersec dotCrossing)
        {
            if (lines.Count == 2)
            {
                LineB = lines[0];
                LineA = lines[0];
            }
            else
                throw new NotSupportedException($"Wrong number lines in {lines}");
            this.BaseDot = baseDot;
            this.DotCrossing = dotCrossing;
        }

        internal List<LineConnection> TryLinkSolutionWithEdgeDonCrossBordersRect(Edge edge)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Line> GetListLines()
        {
            yield return LineB;
            yield return LineA;
        }
    }


    public class SolutionForDot : ISolution
    {

        Solution _SolutionDotA;
        int _numLastEdge;

        int ISolution.NumEdge => _numLastEdge;

        public SolutionForDot(Solution dotA, int numLastEdge)
        {
            _SolutionDotA = dotA;
            _numLastEdge = numLastEdge;
        }

        IEnumerable<Solution> ISolution.GetListSolution()
        {
            yield return _SolutionDotA;
        }

        public IEnumerable<Line> GetListLinesFromSolution()
        {
            yield return _SolutionDotA.LineB;
            yield return _SolutionDotA.LineA;
        }

        //internal int GetIdxLastCrossingEdgeFromEndPoint()
        //{
        //    return _numLastEdge;
        //}

        internal List<Vector2> GetListEdgeDotsLastCrossingEdge()
        {
            //Dots on Edge where LineB & LineA cross this edge
            throw new NotImplementedException();
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            yield return _SolutionDotA.BaseDot;
        }
    }

    public class SolutionForEdge : ISolution
    {
        private Solution _SolutionDotA;
        private Solution _SolutionDotB;  //for horizontal edge lineA.dot.x <  lineB.dot.x for vertical lineA.dot.y <  lineB.dot.y 
        private int _numEdge;
        private Vector2 _startPointFindPath;

        int ISolution.NumEdge => this._numEdge;

        public SolutionForEdge(Vector2 startPointFindPath)
        {

        }

        public SolutionForEdge()
        {
        }

        IEnumerable<Solution> ISolution.GetListSolution()
        {
            yield return _SolutionDotB;
            yield return _SolutionDotA;
        }

        public IEnumerable<Line> GetListLinesFromSolution()
        {
            yield return _SolutionDotB.LineB;
            yield return _SolutionDotB.LineA;
            yield return _SolutionDotA.LineB;
            yield return _SolutionDotA.LineA;
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            yield return _SolutionDotB.BaseDot;
            yield return _SolutionDotA.BaseDot;
        }
    }
}