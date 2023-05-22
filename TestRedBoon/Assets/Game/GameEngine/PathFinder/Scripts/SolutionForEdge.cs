using System.Collections.Generic;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class SolutionForEdge : ISolution
    {
        private Solution _SolutionDotA;
        private Solution _SolutionDotB;  //for horizontal edge lineA.dot.x <  lineB.dot.x for vertical lineA.dot.y <  lineB.dot.y 
        private int _numEdge;
        private Vector2 _startPointFindPath;

        int ISolution.NumEdge => this._numEdge;

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
            yield return _SolutionDotB.IntersecBaseDot.dot;
            yield return _SolutionDotA.IntersecBaseDot.dot;
        }
    }
}