using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GMTools.Math;
using GameEngine.Environment;

namespace GameEngine.PathFinder
{
    public class SolutionForEdgeForStartPoint : ISolution
    {
        private readonly SectorSolutions[] _arrSectorSolutions;
        private readonly int _numLastCrossedEdge;
        private readonly int _numRecBaseDot;
        private readonly ConnectionDot[] _arrConnectionDot;

        private const int NUMNEWSOLUTIONS = 4;

        int ISolution.NumLastCrossedEdgeBySolution => _numLastCrossedEdge;

        int ISolution.NumRecBaseDotSolution => _numRecBaseDot;

        public SolutionForEdgeForStartPoint(SectorSolutions[] arrSectorSolutions, ConnectionDot[] arrConnectionDot, int numLastEdge, int numRecBaseDot)
        {
            _arrSectorSolutions = arrSectorSolutions;
            _arrConnectionDot = arrConnectionDot;
            _numLastCrossedEdge = numLastEdge;
            _numRecBaseDot = numRecBaseDot;
        }

        IEnumerable<SectorSolutions> ISolution.GetListSectorSolutions()
        {
            for (int i = 0; i < _arrSectorSolutions.Length; i++)
                yield return _arrSectorSolutions[i];
        }

        public IEnumerable<Line> GetListLinesFromSectorSolutions()
        {
            for (int i = 0; i < _arrSectorSolutions.Length; i++)
            {
                yield return _arrSectorSolutions[i].LineB;
                yield return _arrSectorSolutions[i].LineA;
            }
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            for (int i = 0; i < _arrSectorSolutions.Length; i++)
                yield return _arrSectorSolutions[i].baseDotSectorSolutions;
        }

        IEnumerable<ConnectionDot> ISolution.GetListConnectionDotsSolution()
        {
            for (int i = 0; i < _arrConnectionDot.Length; i++)
                yield return _arrConnectionDot[i];
        }


        internal static SolutionForEdgeForStartPoint FindAndCreateNewSolutionForEdgeForStartPoint(ISolution solutionObjForStartPoint, int farthestNumEdge)
        {
            int numEdgeCurrentSolution = solutionObjForStartPoint.NumLastCrossedEdgeBySolution;
            if (numEdgeCurrentSolution == farthestNumEdge)
                //It means that the intial Data contain the problem which we not detected at checking it
                throw new NotSupportedException("Something wrong, because in this case the Finder.IsBothSolutionOnOneEdge() should have been called before");

            //we trying to find solution on next edge after Edge current solution
            int closestNumEdge = numEdgeCurrentSolution + 1;
            Debug.LogWarning($"SolutionForEdge(SolutionSide[{SolutionSide.Start}], closeEdge[{numEdgeCurrentSolution}], farEdge[{farthestNumEdge}])");

            int numRecBaseDot = StoreInfoEdges.GetNumRectWithEdgeForSolution(numEdgeCurrentSolution, SolutionSide.Start);
            List<Line>[] arrlistLines = new List<Line>[2] { new List<Line>(2), new List<Line>(2) };

            List<ConnectionDot> arrDotPathsSectorSolutions = solutionObjForStartPoint.GetListConnectionDotsSolution().ToList();
            Vector2[] newBaseDotsSectorSolutions = StoreInfoEdges.GetListDotsEdge(numEdgeCurrentSolution).ToArray();

            //we trying to find solution on next edge after Edge current solution
            foreach ((int currentTestingNumEdge, int nextEdgeAfterCurrentWhereTakenDots) in StoreInfoEdges.GetOrderedListNumEdges(closestNumEdge, farthestNumEdge))
            {
                Debug.Log($"Trying link with Edge[{currentTestingNumEdge}]");
                for (int numBaseDot = 0; numBaseDot < newBaseDotsSectorSolutions.Length; numBaseDot++)
                {
                    foreach (Vector2 dotEdge in StoreInfoEdges.GetListDotsEdge(currentTestingNumEdge))
                    {
                        (bool isPassedEdges, Line lineBTWBaseDotAndEdge, _) = Line.TryLinkTwoDotsThroughEdges(newBaseDotsSectorSolutions[numBaseDot], dotEdge,
                            closestNumEdge, nextEdgeAfterCurrentWhereTakenDots);
                        if (isPassedEdges)
                        {
                            arrlistLines[numBaseDot].Add(lineBTWBaseDotAndEdge);
                        }
                    }
                }
                int countLinesBaseDotA = arrlistLines[0].Count();
                int countLinesBaseDotB = arrlistLines[1].Count();
                Debug.Log($"Was found {countLinesBaseDotA} LinkLines the current BaseDotA {newBaseDotsSectorSolutions[0]} with Edge[{currentTestingNumEdge}] ");
                Debug.Log($"Was found {countLinesBaseDotB} LinkLines the current BaseDotB {newBaseDotsSectorSolutions[1]} with Edge[{currentTestingNumEdge}] ");
                switch (arrlistLines.Count())
                {
                    case 1:
                    case 2:
                    case 3:
                        Debug.Log("SKIPPED: Solution HARD CASE");
                        break;
                    case 4:
                        Debug.Log("Will create new {Solution}");
                        return CreateSolutionForEdge(numRecBaseDot, arrlistLines, newBaseDotsSectorSolutions, currentTestingNumEdge);
                    default:
                        break;
                }
                arrlistLines[0].Clear();
                arrlistLines[1].Clear();
            }
            for (int numBaseDot = 0; numBaseDot < newBaseDotsSectorSolutions.Length; numBaseDot++)
            {
                foreach (Vector2 dotEdge in StoreInfoEdges.GetListDotsEdge(closestNumEdge))
                {
                    //link the baseDotSolution with the same edge where it exist, it will always be possible
                    arrlistLines[numBaseDot].Add(Line.CreateLine(newBaseDotsSectorSolutions[numBaseDot], dotEdge));
                }
            }
            return CreateSolutionForEdge(numRecBaseDot, arrlistLines, newBaseDotsSectorSolutions, closestNumEdge);
        }

        private static SolutionForEdgeForStartPoint CreateSolutionForEdge(int numRecBaseDot, List<Line>[] arrlistLines, Vector2[] newBaseDotsSectorSolutions,
            int currentTestingNumEdge)
        {
            DebugFinder.DebugTurnOn(active: true);
            DebugFinder.DebugDrawLine(arrlistLines[0], $"Solution1 ForEdge[{currentTestingNumEdge}]");
            DebugFinder.DebugDrawLine(arrlistLines[1], $"Solution2 ForEdge[{currentTestingNumEdge}]");
            DebugFinder.DebugTurnOn(active: false);
            SectorSolutions[] arrSectorSolutions = new SectorSolutions[]
            {
                            new SectorSolutions(arrlistLines[0], newBaseDotsSectorSolutions[0]),
                            new SectorSolutions(arrlistLines[1], newBaseDotsSectorSolutions[1])
            };
            ConnectionDot[] arrConnectionDots = new ConnectionDot[]
            {
                            new ConnectionDot(newBaseDotsSectorSolutions[0], null),
                            new ConnectionDot(newBaseDotsSectorSolutions[1], null)
            };
            return new SolutionForEdgeForStartPoint(arrSectorSolutions, arrConnectionDots, currentTestingNumEdge, numRecBaseDot);
        }
    }
}