using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class SolutionForDot : ISolution
    {

        private readonly Solution _SolutionDotA;
        private readonly int _numLastCrossedEdge;
        private readonly int _numRecBaseDot;

        int ISolution.NumLastCrossedEdgeEdge => _numLastCrossedEdge;

        int ISolution.NumRecBaseDotSolution => _numRecBaseDot;

        public SolutionForDot(Solution solution, int numLastEdge, int numRecBaseDot)
        {
            _SolutionDotA = solution;
            _numLastCrossedEdge = numLastEdge;
            //Rec will before last crossed Edge
            _numRecBaseDot = numRecBaseDot;
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