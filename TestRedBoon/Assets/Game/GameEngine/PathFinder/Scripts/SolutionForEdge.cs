using System.Collections.Generic;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class SolutionForEdge : ISolution
    {
        private SectorSolutions _SolutionDotA;
        private SectorSolutions _SolutionDotB;  //for horizontal edge lineA.dot.x <  lineB.dot.x for vertical lineA.dot.y <  lineB.dot.y 
        private int _numEdge;
        private Vector2 _startPointFindPath;
        private readonly int _numRecBaseEdge;

        int ISolution.NumLastCrossedEdgeBySolution => this._numEdge;

        int ISolution.NumRecBaseDotSolution => _numRecBaseEdge;

        public SolutionForEdge()
        {
            throw new System.NotImplementedException("SolutionForEdge.ctor()");
            //Rec will !!! after last crossed Edge !!!
            //_numRecBaseDot = (solutionSide == SolutionSide.Start)
            //    ? StoreInfoEdges.GetNumRect(numLastEdge, RecType.SecondRect) : StoreInfoEdges.GetNumRect(numLastEdge, RecType.FirstRect);
        }

        IEnumerable<SectorSolutions> ISolution.GetListSectorSolutions()
        {
            yield return _SolutionDotB;
            yield return _SolutionDotA;
        }

        public IEnumerable<Line> GetListLinesFromSectorSolutions()
        {
            yield return _SolutionDotB.LineB;
            yield return _SolutionDotB.LineA;
            yield return _SolutionDotA.LineB;
            yield return _SolutionDotA.LineA;
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            yield return _SolutionDotB.IntersecBaseDot.dot;
            yield return _SolutionDotA.IntersecBaseDot.dot;
        }
    }
}