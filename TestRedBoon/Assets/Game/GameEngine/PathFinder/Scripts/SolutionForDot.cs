using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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
            yield return _sectorSolutionsDotA.connectionDot.dot;
        }

        IEnumerable<ConnectionDot> ISolution.GetListDotPathSectorSolutions()
        {
            yield return _sectorSolutionsDotA.connectionDot;
        }

        internal static SolutionForDot CreateSolutionForDot(Vector2 baseDotSolution, int closestNumEdge, int farthestNumEdge, SolutionSide solutionSide)
        {
            Debug.LogWarning($"CreateSolutionForDot(SolutionSide[{solutionSide}], closeEdge[{closestNumEdge}], farEdge[{farthestNumEdge}])");
            int numRecBaseDot = StoreInfoEdges.GetNumRectWithEdgeForSolution(closestNumEdge, solutionSide);
            List<Line> listLines = new List<Line>(2);
            foreach ((int currentTestingNumEdge, int nextEdgeAftercurrent) in StoreInfoEdges.GetOrderedListNumEdges(closestNumEdge, farthestNumEdge))
            {
                Debug.Log($"Trying link with Edge[{currentTestingNumEdge}]");

                foreach (Vector2 dotEdge in StoreInfoEdges.GetListDotsEdge(currentTestingNumEdge))
                {
                    //Test possibility to create line with baseDot and dot from other edge with can pass all edges between them
                    //int nextEdgeAftercurrent = currentTestingNumEdge + step;    will skip current edge fot testing pass
                    (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = Line.TryLinkTwoDotsThroughEdges(baseDotSolution, dotEdge, closestNumEdge,
                        nextEdgeAftercurrent);
                    if (isPassedEdges)
                    {
                        listLines.Add(lineBTWBaseDotAndEdge);
                    }
                }
                Debug.Log($"Was found {listLines.Count()} LinkLines the current {baseDotSolution} with Edge[{currentTestingNumEdge}] ");
                switch (listLines.Count())
                {
                    case 1:
                        Debug.Log("SKIPPED: Solution HARD CASE");
                        break;
                    case 2:
                        Debug.Log("Will create new {Solution}");
                        DebugFinder.DebugTurnOn(active: true);
                        DebugFinder.DebugDrawLine(listLines, $"Solution ForEdge[{currentTestingNumEdge}]");
                        DebugFinder.DebugTurnOn(active: false);
                        return new SolutionForDot(new SectorSolutions(listLines, new ConnectionDot(baseDotSolution, new List<ConnectionDot>() )), currentTestingNumEdge, numRecBaseDot);
                    default:
                        break;
                }
                listLines.Clear();
            }
            foreach (Vector2 dotEdge in StoreInfoEdges.GetListDotsEdge(closestNumEdge))
            {
                //in case trying to link the baseDotSolution with the same edge where it exist, it will always be possible
                listLines.Add(Line.CreateLine(baseDotSolution, dotEdge));
            }
            Debug.Log("Will create a new {Solution} on closest Edge");
            DebugFinder.DebugTurnOn(active: true);
            DebugFinder.DebugDrawLine(listLines, $"Solution ForEdge[{closestNumEdge}]");
            DebugFinder.DebugTurnOn(active: false);
            return new SolutionForDot(new SectorSolutions(listLines, new ConnectionDot(baseDotSolution, new List<ConnectionDot>())), closestNumEdge, numRecBaseDot);
        }
    }
}