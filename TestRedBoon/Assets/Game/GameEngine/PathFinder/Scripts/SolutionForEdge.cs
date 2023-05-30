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
        private readonly SectorSolutions _sectorSolutionsDotA;
        private readonly SectorSolutions _sectorSolutionsDotB;  //for horizontal edge lineA.dot.x <  lineB.dot.x for vertical lineA.dot.y <  lineB.dot.y 
        private readonly int _numLastCrossedEdge;
        private readonly int _numRecBaseDot;

        private const int NUMNEWSOLUTIONS = 4;

        int ISolution.NumLastCrossedEdgeBySolution => _numLastCrossedEdge;

        int ISolution.NumRecBaseDotSolution => _numRecBaseDot;

        public SolutionForEdgeForStartPoint(SectorSolutions sectorSolutionsDotA, SectorSolutions sectorSolutionsDotB, int numLastEdge, int numRecBaseDot)
        {
            //Rec will !!! after last crossed Edge !!!
            //_numRecBaseDot = (solutionSide == SolutionSide.Start)
            //    ? StoreInfoEdges.GetNumRect(numLastEdge, RecType.SecondRect) : StoreInfoEdges.GetNumRect(numLastEdge, RecType.FirstRect);
            _sectorSolutionsDotA = sectorSolutionsDotA;
            _sectorSolutionsDotB = sectorSolutionsDotB;
            _numLastCrossedEdge = numLastEdge;
            _numRecBaseDot = numRecBaseDot;
        }

        IEnumerable<SectorSolutions> ISolution.GetListSectorSolutions()
        {
            yield return _sectorSolutionsDotB;
            yield return _sectorSolutionsDotA;
        }

        public IEnumerable<Line> GetListLinesFromSectorSolutions()
        {
            yield return _sectorSolutionsDotB.LineB;
            yield return _sectorSolutionsDotB.LineA;
            yield return _sectorSolutionsDotA.LineB;
            yield return _sectorSolutionsDotA.LineA;
        }

        IEnumerable<Vector2> ISolution.GetListBasedDotsSolution()
        {
            yield return _sectorSolutionsDotB.connectionDot.dot;
            yield return _sectorSolutionsDotA.connectionDot.dot;
        }

        IEnumerable<ConnectionDot> ISolution.GetListDotPathSectorSolutions()
        {
            yield return _sectorSolutionsDotB.connectionDot;
            yield return _sectorSolutionsDotA.connectionDot;
        }


        internal static SolutionForEdgeForStartPoint CreateNewSolutionForEdge(ISolution solutionObjForStartPoint, int farthestNumEdge)
        {
            int numNewSolutions = 0;
            int numEdgeCurrentSolution = solutionObjForStartPoint.NumLastCrossedEdgeBySolution;
            if (numEdgeCurrentSolution == farthestNumEdge)
                throw new NotSupportedException("Something wrong, because in this case must be called Finder.IsBothSolutionOnOneEdge()");

            //we trying to find solution on next edge after Edge current solution
            int closestNumEdge = numEdgeCurrentSolution + 1;
            Debug.LogWarning($"SolutionForEdge(SolutionSide[{SolutionSide.Start}], closeEdge[{numEdgeCurrentSolution}], farEdge[{farthestNumEdge}])");

            int numRecBaseDot = StoreInfoEdges.GetNumRectWithEdgeForSolution(numEdgeCurrentSolution, SolutionSide.Start);
            List<Line>[] arrlistLines = new List<Line>[2] { new List<Line>(2), new List<Line>(2) };

            List<ConnectionDot> arrDotPathsSectorSolutions = solutionObjForStartPoint.GetListDotPathSectorSolutions().ToList();
            Vector2[] newBaseDotsSectorSolutions = StoreInfoEdges.GetListDotsEdge(numEdgeCurrentSolution).ToArray();

            //we trying to find solution on next edge after Edge current solution
            foreach ((int currentTestingNumEdge, int nextEdgeAfterCurrentWhereTakenDots) in StoreInfoEdges.GetOrderedListNumEdges(closestNumEdge, farthestNumEdge))
            {
                Debug.Log($"Trying link with Edge[{currentTestingNumEdge}]");
                for (int numBaseDot = 0; numBaseDot < newBaseDotsSectorSolutions.Length; numBaseDot++)
                {
                    foreach (Vector2 dotEdge in StoreInfoEdges.GetListDotsEdge(currentTestingNumEdge))
                    {
                        (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = Line.TryLinkTwoDotsThroughEdges(newBaseDotsSectorSolutions[numBaseDot], dotEdge,
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
                        DebugFinder.DebugTurnOn(active: true);
                        DebugFinder.DebugDrawLine(arrlistLines[0], $"SolutionDotA ForEdge[{currentTestingNumEdge}]");
                        DebugFinder.DebugDrawLine(arrlistLines[1], $"SolutionDotB ForEdge[{currentTestingNumEdge}]");
                        DebugFinder.DebugTurnOn(active: false);
                        return new SolutionForEdgeForStartPoint(new SectorSolutions(arrlistLines[0], new ConnectionDot(newBaseDotsSectorSolutions[0], null)),
                            new SectorSolutions(arrlistLines[1], new ConnectionDot(newBaseDotsSectorSolutions[1], null)), currentTestingNumEdge, numRecBaseDot);
                    default:
                        break;
                }
                arrlistLines[0].Clear();
                arrlistLines[1].Clear();
            }

            IEnumerable<Vector2> baseDotsCurrentSolution = solutionObjForStartPoint.GetListBasedDotsSolution();
            foreach (SectorSolutions currentSolution in solutionObjForStartPoint.GetListSectorSolutions())
            {
                ConnectionDot prev = currentSolution.connectionDot;
                Vector2 baseDotCurrentSolutions = currentSolution.connectionDot.dot;

                for (int numEdge = farthestNumEdge; numEdge < numEdgeCurrentSolution; numEdge--)
                {
                    //Edge edge = _arredges[numEdge];
                    Edge edge = new Edge();
                    foreach (Vector2 basedot in GetListBaseDotFromEdge(edge))
                    {
                        foreach (Vector2 currentBaseDotSolutionin in baseDotsCurrentSolution)
                        {
                            SectorSolutions newSolutionForEdge = TryFindNewSolutionForEdge(currentBaseDotSolutionin, basedot, prev, out Vector2 dotCrossing);

                            if (newSolutionForEdge != null)
                            {
                                //_listDotsPath.AddDotCross(newSolutionForEdge.BaseDotIntersec, prev);
                                numNewSolutions++;
                            }
                        }

                    }
                    if (numNewSolutions == NUMNEWSOLUTIONS)
                    {
                        Debug.Log("We Found New Solution more close to EndPath Can repeat External Cycle");
                        //return new SolutionForEdgeFromStartPoint();
                    }
                    else
                    {
                        Debug.Log("We Must select ednge more close to StartPsth and try to Found New Solution on this Edge");
                    }
                }
            }

            throw new System.NotImplementedException();
        }

        private static SectorSolutions TryFindNewSolutionForEdge(Vector2 currentBaseDotSolutionin, Vector2 basedot, ConnectionDot prev, out Vector2 dotCrossing)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Vector2> GetListBaseDotFromEdge(Edge edge)
        {
            throw new NotImplementedException();
        }
    }
}