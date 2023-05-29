using System;
using System.Collections.Generic;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class SolutionForDot : ISolution
    {

        private readonly SectorSolutions _sectorSolutionsDotA;
        private readonly int _numLastCrossedEdge;
        private readonly int _numRecBaseDot;

        int ISolution.NumLastCrossedEdgeBySolution => _numLastCrossedEdge;

        int ISolution.NumRecBaseDotSolution => _numRecBaseDot;

        public SolutionForDot(SectorSolutions sectorSolutions, int numLastEdge, int numRecBaseDot)
        {
            _sectorSolutionsDotA = sectorSolutions;
            _numLastCrossedEdge = numLastEdge;
            //Rec will before last crossed Edge
            _numRecBaseDot = numRecBaseDot;
        }

        IEnumerable<SectorSolutions> ISolution.GetListSectorSolutions()
        {
            yield return _sectorSolutionsDotA;
        }

        public IEnumerable<Line> GetListLinesFromSectorSolutions()
        {
            yield return _sectorSolutionsDotA.LineB;
            yield return _sectorSolutionsDotA.LineA;
        }

        internal List<Vector2> GetListEdgeDotsLastCrossingEdge()
        {
            //Dots on Edge where LineB & LineA cross this edge
            throw new NotImplementedException();
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            yield return _sectorSolutionsDotA.IntersecBaseDot.dot;
        }
    }
}