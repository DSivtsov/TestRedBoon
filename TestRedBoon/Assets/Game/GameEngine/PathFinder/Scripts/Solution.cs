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
    }

    public class Solution // angleLineB > angleLineA in degrees // in k factor
    {
        public readonly Line LineB;
        public readonly Line LineA;
        public readonly Vector2 BaseDot;
        public readonly DotCross DotCrossing;

        public Solution(List<Line> lines, Vector2 baseDot, DotCross dotCrossing)
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

        public SolutionForDot(Solution dotA, int numLastEdge)
        {
            _SolutionDotA = dotA;
            _numLastEdge = numLastEdge;
        }

        //public SolutionForDot(Vector2 startPointFindPath)
        //{
        //    _dotA = new Solution(startPointFindPath);
        //}

        IEnumerable<Solution> ISolution.GetListSolution()
        {
            yield return _SolutionDotA;
        }

        internal List<LineConnection> TryLinkSolutionWithOtherDotSolution(SolutionForDot solutionEndPath)
        {
            //Is Crossing in EdgePoints
            //LineConnection[] GetLineto

            //Is Crossing on Line was going from EdgePoints
            throw new NotImplementedException();
        }

        public IEnumerable<Line> GetListLinesFromSolution()
        {
            yield return _SolutionDotA.LineB;
            yield return _SolutionDotA.LineA;
        }

        internal int GetIdxLastCrossingEdgeFromEndPoint()
        {
            return _numLastEdge;
        }

        internal List<Vector2> GetListEdgeDotsLastCrossingEdge()
        {
            //Dots on Edge where LineB & LineA cross this edge
            throw new NotImplementedException();
        }
    }

    public class SolutionForEdge : ISolution
    {
        private Solution _SolutionDotA;
        private Solution _SolutionDotB;  //for horizontal edge lineA.dot.x <  lineB.dot.x for vertical lineA.dot.y <  lineB.dot.y 
        private int _numEdge;
        private Vector2 _startPointFindPath;

        public int NumEdge => this._numEdge;

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

        //public IEnumerable<Solution> GetListSolutions()
        //{
        //    yield return _SolutionDotB;
        //    yield return _SolutionDotA;
        //}

        public IEnumerable<Vector2> GetListBasedDotsSolution()
        {
            yield return _SolutionDotB.BaseDot;
            yield return _SolutionDotA.BaseDot;
        }
    }
}