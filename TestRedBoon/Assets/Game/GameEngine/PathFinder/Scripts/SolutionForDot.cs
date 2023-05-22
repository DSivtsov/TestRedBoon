using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class SolutionForDot : ISolution
    {

        Solution _SolutionDotA;
        int _numLastEdge;

        int ISolution.NumEdge => _numLastEdge;

        public SolutionForDot(Solution solution, int numLastEdge)
        {
            _SolutionDotA = solution;
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

        internal List<Vector2> GetListEdgeDotsLastCrossingEdge()
        {
            //Dots on Edge where LineB & LineA cross this edge
            throw new NotImplementedException();
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            yield return _SolutionDotA.IntersecBaseDot.dot;
        }
    }
}