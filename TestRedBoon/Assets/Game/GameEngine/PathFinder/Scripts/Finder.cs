using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.Environment;
using GMTools.Math;


namespace GameEngine.PathFinder
{
    public class Finder : IPathFinder
    {
        private const int NumDotCrossinsInCaseSimpleTurn = 4;
        private const int NUMNEWSOLUTIONS = 4;
        private const int NumDotHaveCrossingwithEndPathInCaseDirectConnection = 1;
        private const int FirstNumberEdge = 0;
        private int _lastNumberEdge;
        private Edge[] _arredges;
        private ListIntersec _listDotCrossing;
        private ISolution _solutionForEndPoint;
        private Vector2 _baseDotSolutionEnd;
        private int _numRecBaseDotEnd;
        private List<Line> _listLinesSolutionEnd;
        private int _numLastCrossingEdgeFromSolutionEnd;
        private Rectangle _closestRectAccessableFromSolutionEnd;
        private int _moreClosestFromStartEdgeToRectAccessableFromEndPoint;

        IEnumerable<Vector2> IPathFinder.GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            DebugFinder.InitDebugFinder(active: false);

            _arredges = edges.ToArray();
            _lastNumberEdge = _arredges.Length - 1;
            StoreInfoEdges.InitStoreEdges(_arredges);

            _listDotCrossing = new ListIntersec(_arredges.Length);
            _listDotCrossing.AddDotCross(startPointFindPath, null);

            (bool isPassedEdges, _, _) = TryLinkTwoDotsThroughEdges(startPointFindPath, endPointFindPath, 0, _lastNumberEdge);
            if (isPassedEdges)
            {
                Debug.Log($"We found the direct Line from StartPath to EndPath without any turns");
                //It means that we have found a Path all dots in _listDotCrossing
                _listDotCrossing.SaveDataLastConnectionsWithEndPath(NumDotHaveCrossingwithEndPathInCaseDirectConnection, endPointFindPath);
                return _listDotCrossing.GetPath();
            }

            Debug.LogWarning($"CreateSolutionForDot(endPointFindPath, closeEdge[{_lastNumberEdge}], farEdge[{FirstNumberEdge}])");

            _solutionForEndPoint = CreateSolutionForDot(endPointFindPath, _lastNumberEdge, FirstNumberEdge,
                StoreInfoEdges.GetNumRectWithEdgeForSolution(_lastNumberEdge, SolutionSide.End));

            _baseDotSolutionEnd = endPointFindPath;
            _numRecBaseDotEnd = _solutionForEndPoint.NumRecBaseDotSolution;
            _listLinesSolutionEnd = _solutionForEndPoint.GetListLinesFromSectorSolutions().ToList();
            _numLastCrossingEdgeFromSolutionEnd = _solutionForEndPoint.NumLastCrossedEdgeEdge;
            _closestRectAccessableFromSolutionEnd = _arredges[_numLastCrossingEdgeFromSolutionEnd].First;

            Debug.Log($"SolutionForDot(endPointFindPath): _listLinesEndPoint[{_listLinesSolutionEnd.Count}] _idxLastCrossingEdgeFromEndPoint[{_numLastCrossingEdgeFromSolutionEnd}]" +
                $" _closestRectAccessableFromEndPoint[{_closestRectAccessableFromSolutionEnd.Min},{_closestRectAccessableFromSolutionEnd.Max}]");

            int numDotHaveCrossingwithEndPoint;

            Debug.LogWarning($"CreateSolutionForDot(startPointFindPath, closeEdge[{FirstNumberEdge}], farEdge[{_lastNumberEdge}])");

            ISolution currentSolutionFromStartPath = CreateSolutionForDot(startPointFindPath, FirstNumberEdge, _lastNumberEdge,
                StoreInfoEdges.GetNumRectWithEdgeForSolution(FirstNumberEdge, SolutionSide.Start));

            if (_numLastCrossingEdgeFromSolutionEnd == currentSolutionFromStartPath.NumLastCrossedEdgeEdge)
            {
                Debug.Log($"We found the Both Solution have dots on one Edge");
                //Both Solution have dots on one Edge, the can be SolutionForDot (2 DotCross) and SolutionForEdfe (4 DotCross)
                numDotHaveCrossingwithEndPoint = BothSolutionHaveDotsOnOneEdge(currentSolutionFromStartPath);
                _listDotCrossing.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, endPointFindPath);
                return _listDotCrossing.GetPath();
            }

            Debug.LogWarning("FindCrossingCurrentSolutionWithSolutionForEndPoint(currentSolutionFromStartPath)");

            numDotHaveCrossingwithEndPoint = FindCrossingCurrentSolutionWithSolutionForEndPoint(currentSolutionFromStartPath);
            if (numDotHaveCrossingwithEndPoint != 0)
            {
                Debug.Log($"We found the crossing the line from PointEndPath with Line from currentSolution");
                //It means that we have found a Path all dots in _listDotCrossing
                _listDotCrossing.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, endPointFindPath);
                return _listDotCrossing.GetPath();
            }

            Debug.LogWarning("CreateDirectLineLinkedCurrentSolutionWithSolutionForEndPoint(currentSolutionFromStartPath)");

            numDotHaveCrossingwithEndPoint = CreateDirectLineLinkedCurrentSolutionWithSolutionForEndPoint(currentSolutionFromStartPath);
            if (numDotHaveCrossingwithEndPoint != 0)
            {
                Debug.Log($"We found [{numDotHaveCrossingwithEndPoint}] direct lines between the edge of currentSolution and the solution for PointEndPath");
                //It means that we have found a direct lines between the edge of currentSolution and the solution for PointEndPath
                _listDotCrossing.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, endPointFindPath);
                return _listDotCrossing.GetPath();
            }
            throw new Exception("Create SOlution For Edge DISABLED");
            {
                //Begin Find Line btw current Solution and most closest Rect to _closestRectAccessableFromEndPoint
                //FindCrossingCurrentSolutionWithEdge(currentSolutionFromStartPath);
            }
            throw new System.NotImplementedException("NotImplementedException");
        }

        private int CreateDirectLineLinkedCurrentSolutionWithSolutionForEndPoint(ISolution solutionObjStart)
        {
            int numNewDotCorssing = 0;
            List<Line> listLines = new List<Line>(8);
            int numEdgeCurrentSolutionStart = solutionObjStart.NumLastCrossedEdgeEdge;
            foreach (SectorSolutions currentSectorSolutionsStart in solutionObjStart.GetListSectorSolutions())
            {
                DotIntersec prev = currentSectorSolutionsStart.IntersecBaseDot;
                Debug.Log($"Trying link with Edge[{numEdgeCurrentSolutionStart}]");

                foreach (Vector2 dotEdgeStart in StoreInfoEdges.GitListDotsEdge(numEdgeCurrentSolutionStart))
                {
                    foreach (var dotEdgeEnd in StoreInfoEdges.GitListDotsEdge(_numLastCrossingEdgeFromSolutionEnd))
                    {
                        (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = TryLinkTwoDotsThroughEdges(dotEdgeStart, dotEdgeEnd, numEdgeCurrentSolutionStart,
                            _numLastCrossingEdgeFromSolutionEnd);
                        if (isPassedEdges)
                        {
                            listLines.Add(lineBTWBaseDotAndEdge);
                            _listDotCrossing.AddDotCross(dotEdgeStart, prev);
                            numNewDotCorssing ++;
                        } 
                    }
                }

            }
            return numNewDotCorssing;
        }

        private int FindCrossingCurrentSolutionWithSolutionForEndPoint(ISolution solutionObjStart)
        {
            int numNewDotCorssing = 0;
            //int count = 0;
            DebugFinder.DebugTurnOn(true);
            int numLastCrossingEdgeFromStart = solutionObjStart.NumLastCrossedEdgeEdge;
            int numRecBaseDotStart = solutionObjStart.NumRecBaseDotSolution;
            Debug.Log($"idxLastCrossingEdgeFromEndPoint[{_numLastCrossingEdgeFromSolutionEnd}] idxLastCrossingEdgeFromCurrentSolution[{numLastCrossingEdgeFromStart}]");
            foreach (SectorSolutions currentSolutionStart in solutionObjStart.GetListSectorSolutions())
            {
                Vector2 baseDotStart = currentSolutionStart.BaseDotIntersec;
                
                DotIntersec prev = currentSolutionStart.IntersecBaseDot;
                foreach (Line lineSolutionStart in currentSolutionStart.GetListLines())
                {
                    foreach (Line lineSolutionEnd in _listLinesSolutionEnd)
                    {
                        (bool isCrossing, Vector2 dotCrossing) = CrossLineSolutionWithLine(lineSolutionStart, lineSolutionEnd);
                        if (isCrossing)
                        {
                            if (StoreInfoEdges.IsDotCrossingBetweenBaseDotAndEdge(dotCrossing, baseDotStart, numLastCrossingEdgeFromStart))
                            {//DotCrossing Between BaseDotStart And LastCrossingEdgeFromCurrentSolution
                                (bool dotInRec, int numRect) = StoreInfoEdges.IsDotInRectBetweenRecBaseDotAndRectEdge(dotCrossing, numRecBaseDotStart, numLastCrossingEdgeFromStart,
                                    SolutionSide.Start);
                                if (IsLinePassEdges(lineSolutionEnd, _numLastCrossingEdgeFromSolutionEnd, numRect, SolutionSide.Start))
                                {
                                    DebugFinder.DebugDrawDot(dotCrossing);
                                    _listDotCrossing.AddDotCross(dotCrossing, prev);
                                    numNewDotCorssing++;
                                }
                            }
                            else if (StoreInfoEdges.IsDotCrossingBetweenBaseDotAndEdge(dotCrossing, _baseDotSolutionEnd, _numLastCrossingEdgeFromSolutionEnd))
                            {//DotCrossing Between _numLastCrossingEdgeFromSolutionEnd and BaseDotEnd
                                (bool dotInRec, int numRect) = StoreInfoEdges.IsDotInRectBetweenRecBaseDotAndRectEdge(dotCrossing, _numRecBaseDotEnd, _numLastCrossingEdgeFromSolutionEnd,
                                    SolutionSide.End);
                                if (IsLinePassEdges(lineSolutionStart, numLastCrossingEdgeFromStart, numRect, SolutionSide.End))
                                {
                                    DebugFinder.DebugDrawDot(dotCrossing);
                                    _listDotCrossing.AddDotCross(dotCrossing, prev);
                                    numNewDotCorssing++;
                                }
                            }
                            else
                            {//Dot cross Between edges Solution Start and End
                                (bool dotInRec, int numRect) = StoreInfoEdges.IsDotInRectBetweenEdges(dotCrossing, numLastCrossingEdgeFromStart, _numLastCrossingEdgeFromSolutionEnd);
                                bool rezLineSolutionStart = (IsLinePassEdges(lineSolutionStart, numLastCrossingEdgeFromStart, numRect, SolutionSide.End));
                                bool rezLineSolutionEnd = IsLinePassEdges(lineSolutionEnd, _numLastCrossingEdgeFromSolutionEnd, numRect, SolutionSide.Start);
                                if (rezLineSolutionStart && rezLineSolutionEnd)
                                {
                                    DebugFinder.DebugDrawDot(dotCrossing);
                                    _listDotCrossing.AddDotCross(dotCrossing, prev);
                                    numNewDotCorssing++;
                                }
                            }
                        }
                    }

                }
            }
            DebugFinder.DebugTurnOn(false);
            Debug.Log($"{this}: numNewDotCorssing[{numNewDotCorssing}]");
            return numNewDotCorssing;
        }

        private bool IsLinePassEdges(Line lineOtherSolution, int numLastCrossingEdgeByLineOtherSolution, int numRectWhereDotCrossing, SolutionSide solutionSide)
        {
            int numEgde = StoreInfoEdges.GetNumEdge(numRectWhereDotCrossing, solutionSide);
            //Debug.Log($"numRectWhereDotCrossing[{numRectWhereDotCrossing}] numEgde={numEgde} numLastCrossingEdgeFromSolution[{numLastCrossingEdgeByLineOtherSolution}] " +
            //    $"solutionSide[{solutionSide}]");
            switch (solutionSide)
            {
                case SolutionSide.Start:
                    return IsLinePassedThroughEdges(lineOtherSolution, numEgde, numLastCrossingEdgeByLineOtherSolution - 1);
                case SolutionSide.End:
                    return IsLinePassedThroughEdges(lineOtherSolution, numLastCrossingEdgeByLineOtherSolution + 1, numEgde);
                default:
                    throw new NotSupportedException($"Value [{solutionSide}] is not supported");
            }
        }

        private int BothSolutionHaveDotsOnOneEdge(ISolution solutionObj)
        {
            int numNewDotCorssing = 0;
            Edge edge = _arredges[solutionObj.NumLastCrossedEdgeEdge];
            foreach (SectorSolutions currentSolution in solutionObj.GetListSectorSolutions())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                _listDotCrossing.AddDotCross(edge.Start, prev);
                _listDotCrossing.AddDotCross(edge.End, prev);
                numNewDotCorssing += 2;
            }
            return numNewDotCorssing;
        }

        private static bool IsLinePassedThroughEdges(Line line, int numEdgeStart, int numEdgeEnd)
        {
            bool directLineBTWdotsExist = true;
            for (int currentNumTestingEdge = numEdgeStart; currentNumTestingEdge <= numEdgeEnd; currentNumTestingEdge++)
            {
                if (!line.TryIntersecLineWithEdge(currentNumTestingEdge))
                {
                    Debug.Log($"Intersec Line Not crossing Edge[{currentNumTestingEdge}]");
                    directLineBTWdotsExist = false;
                    break;
                }
            }
            return directLineBTWdotsExist;
        }

        private SolutionForDot CreateSolutionForDot(Vector2 baseDotSolution, int closestNumEdge, int farthestNumEdge, int numRecBaseDot)
        {
            List<Line> listLines = new List<Line>(2);
            foreach ((int currentTestingNumEdge, int nextEdgeAftercurrent) in GetOrderedListNumEdges(closestNumEdge, farthestNumEdge))
            {
                Debug.Log($"Trying link with Edge[{currentTestingNumEdge}]");

                foreach (Vector2 dotEdge in StoreInfoEdges.GitListDotsEdge(currentTestingNumEdge))
                {
                    //Test possibility to create line with baseDot and dot from other edge with can pass all edges between them
                    //int nextEdgeAftercurrent = currentTestingNumEdge + step;    will skip current edge fot testing pass
                    (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = TryLinkTwoDotsThroughEdges(baseDotSolution, dotEdge, closestNumEdge,
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
                        return new SolutionForDot(new SectorSolutions(listLines, new DotIntersec(baseDotSolution, null)),currentTestingNumEdge, numRecBaseDot);
                    default:
                        break;
                }
                listLines.Clear();
            }
            foreach (Vector2 dotEdge in StoreInfoEdges.GitListDotsEdge(closestNumEdge))
            {
                //in case trying to link the baseDotSolution with the same edge where it exist, it will always be possible
                listLines.Add(new Line(baseDotSolution, dotEdge));
            }
            Debug.Log("Will create a new {Solution} on closest Edge");
            DebugFinder.DebugTurnOn(active: true);
            DebugFinder.DebugDrawLine(listLines, $"Solution ForEdge[{closestNumEdge}]");
            DebugFinder.DebugTurnOn(active: false);
            return new SolutionForDot(new SectorSolutions(listLines, new DotIntersec(baseDotSolution, null)), closestNumEdge, numRecBaseDot);
        }

        /// <summary>
        /// Give order num Edges from farthestNumEdge till closestNumEdge
        /// </summary>
        /// <param name="closestNumEdge"></param>
        /// <param name="farthestNumEdge"></param>
        /// <returns></returns>
        private IEnumerable<(int currentTestingNumEdge, int nextEdgeAfterCurrent)> GetOrderedListNumEdges(int closestNumEdge, int farthestNumEdge)
        {
            int step = (closestNumEdge < farthestNumEdge) ? -1 : 1;
            int numTestedEdge = Math.Abs(closestNumEdge - farthestNumEdge);
            int currentTestingNumEdge, nextEdgeAfterCurrent;
            for (int i = 0; i < numTestedEdge; i++)
            {
                currentTestingNumEdge = farthestNumEdge + i * step;
                nextEdgeAfterCurrent = currentTestingNumEdge + step;
                yield return (farthestNumEdge + i * step, nextEdgeAfterCurrent);
            }
        }

        /// <summary>
        /// Test possibility to create line between dotA and dotB which can pass all edges between them
        /// </summary>
        /// <param name="dotA"></param>
        /// <param name="dotB"></param>
        /// <param name="startNumEdge"></param>
        /// <param name="endNumEdge"></param>
        /// <returns> if linked (true, numberLastPassEdge) otherwise (false, numberLastNotPassEdge)</returns>
        private (bool isPassedEdge, Line line, int numLastTestedEdge) TryLinkTwoDotsThroughEdges(Vector2 dotA, Vector2 dotB, int startNumEdge, int endNumEdge)
        {
            CorrectOrderEdgeNumbers(ref startNumEdge, ref endNumEdge);
            Debug.Log($"TryLinkTwoDots({dotA}, {dotB}), check Edges from [{startNumEdge}] till [{endNumEdge}]");
            Line lineBTWDots = new Line(dotA,dotB);
            bool directLineBTWdotsExist = true;
            int currentNumTestingEdge = startNumEdge;
            for (; currentNumTestingEdge <= endNumEdge; currentNumTestingEdge++)
            {
                if (!lineBTWDots.TryIntersecLineWithEdge(currentNumTestingEdge))
                {
                    DebugFinder.DebugDrawLineSegment(dotA, dotB, $"Not crossing Edge[{currentNumTestingEdge}]");
                    Debug.Log($"Intersec Line Not crossing Edge[{currentNumTestingEdge}]");
                    directLineBTWdotsExist = false;
                    break;
                }
            }
            return (directLineBTWdotsExist, lineBTWDots, currentNumTestingEdge);
        }

        private void CorrectOrderEdgeNumbers(ref int startNumEdge, ref int endNumEdge)
        {
            if (startNumEdge == endNumEdge)
                return;
             //throw new NotImplementedException($"Wrong call TryLinkTwoDotsThroughEdges() the numEdgeAfterDotA[{numEdgeAfterDotA}]==numEdgeBeforeDotB[{numEdgeBeforeDotB}]");
            if (startNumEdge > endNumEdge)
            {
                int temp = endNumEdge;
                endNumEdge = startNumEdge;
                startNumEdge = temp;
            }
        }

        private SolutionForEdge CreateNewSolutionForEdge(ISolution solutionObj)
        {
            int numNewSolutions = 0;
            int numEdgeCurrentSolution = solutionObj.NumLastCrossedEdgeEdge;
            IEnumerable<Vector2> baseDotsCurrentSolution = solutionObj.GetListBasedDotsSolution();
            foreach (SectorSolutions currentSolution in solutionObj.GetListSectorSolutions())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                Vector2 baseDotCurrentSolutions = currentSolution.BaseDotIntersec;

                for (int numEdge = _moreClosestFromStartEdgeToRectAccessableFromEndPoint; numEdge < numEdgeCurrentSolution; numEdge--)
                {
                    Edge edge = _arredges[numEdge];
                    foreach (Vector2 basedot in GetListBaseDotFromEdge(edge))
                    {
                        foreach (Vector2 currentBaseDotSolutionin in baseDotsCurrentSolution)
                        {
                            SectorSolutions newSolutionForEdge = TryFindNewSolutionForEdge(currentBaseDotSolutionin, basedot, prev, out Vector2 dotCrossing);

                            if (newSolutionForEdge != null)
                            {
                                _listDotCrossing.AddDotCross(newSolutionForEdge.BaseDotIntersec, prev);
                                numNewSolutions++;
                            }
                        }

                    }
                    if (numNewSolutions == NUMNEWSOLUTIONS)
                    {
                        Debug.Log("We Found New Solution more close to EndPath Can repeat External Cycle");
                        return new SolutionForEdge();
                    }
                    else
                    {
                        Debug.Log("We Must select ednge more close to StartPsth and try to Found New Solution on this Edge");
                    }
                }
            }

            throw new System.NotImplementedException();
        }

        private SectorSolutions TryFindNewSolutionForEdge(Vector2 currentBaseDotSolutionin, Vector2 basedot, DotIntersec prev, out Vector2 dotCrossing)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Vector2> GetListBaseDotFromEdge(Edge edge)
        {
            throw new NotImplementedException();
        }

        //private void LinkCurrentSolutionBySimpleTurn(ISolution solutionObj, Edge edge)
        //{
        //    foreach (SectorSolutions currentSolution in solutionObj.GetListSectorSolutions())
        //    {
        //        DotIntersec prev = currentSolution.IntersecBaseDot;
        //        _listDotCrossing.AddDotCross(edge.Start, new DotIntersec(edge.Start, prev));
        //        _listDotCrossing.AddDotCross(edge.End, new DotIntersec(edge.End, prev));
        //    }
        //}

        private (bool isCrossing, Vector2 dotCrossing) CrossLineSolutionWithLine(Line currentLineSolution, Line currentLineEndPoint)
        {
            (Matrix2x2 matrix, VectorB2 b) = CreateDataForLinearSystemEquation(currentLineSolution, currentLineEndPoint);
            VectorB2 vector = LinearSystemEquation.GetSolutionLinearSystemEquation(matrix, b);
            if (vector == null)
            {
                Debug.LogError($"Can't find Solution for crossing line({currentLineSolution}) with line ({currentLineEndPoint})");
                return (false, Vector2.zero);
            }
            return (true, (Vector2)vector); 
        }

        private (Matrix2x2 matrixFactors, VectorB2 b) CreateDataForLinearSystemEquation(Line currentLineSolution, Line currentLineEndPoint)
        {
            (float a11, float a12, float b1) = currentLineSolution.GetDataForMatrix2x2();
            (float a21, float a22, float b2) = currentLineEndPoint.GetDataForMatrix2x2();
            return (new Matrix2x2(a11, a12, a21, a22), new VectorB2(b1, b2));
        }
    }
}