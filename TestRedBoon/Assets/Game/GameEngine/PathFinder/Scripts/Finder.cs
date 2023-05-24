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
        //private int _numEdgeCurrentSolution;
        private int _moreClosestFromStartEdgeToRectAccessableFromEndPoint;

        IEnumerable<Vector2> IPathFinder.GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            DebugFinder.InitDebugFinder(active: false);

            _arredges = edges.ToArray();
            _lastNumberEdge = _arredges.Length - 1;
            //Line.InitLineClass(_arredges);
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
            _listLinesSolutionEnd = _solutionForEndPoint.GetListLinesFromSolution().ToList();
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

            /*
             * TO DO
             * Solution for Parallel Current & End Solution.jpg
             * if (numDotHaveCrossingwithEndPoint == 0 && _idxLastCrossingEdgeFromEndPoint == currentSolutionFromStartPath.NumEdge)
             *  GetCrossingDotsEdgeStart with PreviousEndCurrentSolutionFromStartPath
             *  GetCrossingDotsEdgeEnd with Previous_solutionEndPath
             *  Select from GetCrossingDotsEdgeStart - dot will have less ditance to BaseDotEnd
             *  Select from GetCrossingDotsEdgeEnd - dot will have less ditance to BaseDotCurrentSolution
             *  Line these two dots
             *  ???check crossing the _idxLastCrossingEdgeFromEndPoint
             */
            throw new Exception("FindCrossingCurrentSolutionWithEndPoint(currentSolutionFromStartPath) DISABLED");
            //numDotHaveCrossingwithEndPoint = FindCrossingCurrentSolutionWithEndPoint(currentSolutionFromStartPath);
            //if (numDotHaveCrossingwithEndPoint != 0)
            //{
            //    Debug.Log($"We found the crossing the line from PointEndPath with Line from currentSolution");
            //    //It means that we have found a Path all dots in _listDotCrossing
            //    _listDotCrossing.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, endPointFindPath);
            //    return _listDotCrossing.GetPath();
            //}
            //else
            {
                do
                {
                    int _numEdgeCurrentSolution = currentSolutionFromStartPath.NumLastCrossedEdgeEdge;
                    _moreClosestFromStartEdgeToRectAccessableFromEndPoint = _numLastCrossingEdgeFromSolutionEnd - 1;
                    if (_numEdgeCurrentSolution == _moreClosestFromStartEdgeToRectAccessableFromEndPoint)
                    {
                        //LinkCurrentSolutionBySimpleTurn(currentSolutionFromStartPath, _solutionEndPath.GetListEdgeDotsLastCrossingEdge());
                        LinkCurrentSolutionBySimpleTurn(currentSolutionFromStartPath,_arredges[_numEdgeCurrentSolution]);
                        Debug.Log($"We found the crossing the line from PointEndPath with Last Simple Turn from currentSolution");
                        //It means that we have found a Path all dots in _listDotCrossing
                        //int numDotCrossinsInCaseSimpleTurn = currentSolutionFromStartPath is SolutionForDot
                        Debug.Log($"[currentSolutionFromStartPath is SolutionForDot][{currentSolutionFromStartPath is SolutionForDot}]");
                        Debug.Log($"[currentSolutionFromStartPath is SolutionForEdge][{currentSolutionFromStartPath is SolutionForEdge}]");
                        //_listDotCrossing.SaveDataLastConnectionsWithEndPath(NumDotCrossinsInCaseSimpleTurn, endPointFindPath);
                        _listDotCrossing.SaveDataLastConnectionsWithEndPath(2, endPointFindPath);
                        return _listDotCrossing.GetPath();
                    }
                    else
                    {
                        //Begin Find Line btw current Solution and most closest Rect to _closestRectAccessableFromEndPoint
                        FindCrossingCurrentSolutionWithEdge(currentSolutionFromStartPath);
                    } 
                } while (true);
            }
            //}
            throw new System.NotImplementedException("NotImplementedException");
        }

        private int BothSolutionHaveDotsOnOneEdge(ISolution solutionObj)
        {
            int numNewDotCorssing = 0;
            Edge edge = _arredges[solutionObj.NumLastCrossedEdgeEdge];
            foreach (Solution currentSolution in solutionObj.GetListSolution())
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
                //Debug.Log($"currentNumTestingEdge={currentNumTestingEdge}");
                if (!line.TryIntersecLineWithEdge(currentNumTestingEdge))
                {
                    Debug.Log($"Intersec Line Not crossing Edge[{currentNumTestingEdge}]");
                    directLineBTWdotsExist = false;
                    break;
                }
            }
            return directLineBTWdotsExist;
        }

        private int FindCrossingCurrentSolutionWithSolutionForEndPoint(ISolution solutionObjStart)
        {
            int numNewDotCorssing = 0;
            //int count = 0;
            DebugFinder.DebugTurnOn(true);
            int numLastCrossingEdgeFromStart = solutionObjStart.NumLastCrossedEdgeEdge;
            int numRecBaseDotStart = solutionObjStart.NumRecBaseDotSolution;
            Debug.Log($"idxLastCrossingEdgeFromEndPoint[{_numLastCrossingEdgeFromSolutionEnd}] idxLastCrossingEdgeFromCurrentSolution[{numLastCrossingEdgeFromStart}]");
            foreach (Solution currentSolutionStart in solutionObjStart.GetListSolution())
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
                            //Debug.Log($"dotCrossing_{count} {dotCrossing}");
                            //DebugFinder.DebugDrawDot(dotCrossing, $"dotCrossing_{count}");
                            //count++;
                            
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
                                //Debug.Log($"dotInRec[{dotInRec}] numRect[{numRect}]");
                                //Debug.Log("forLineFromSTart");
                                bool rezLineSolutionStart = (IsLinePassEdges(lineSolutionStart, numLastCrossingEdgeFromStart, numRect, SolutionSide.End));
                                //Debug.Log("forLineFromEnd");
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

        private List<Rectangle> GetListPossibleRect(int idxLastCrossingEdgeFromEndPoint, int idxLastCrossingEdgeFromCurrentSolution)
        {
            List<Rectangle> list = new List<Rectangle>(idxLastCrossingEdgeFromCurrentSolution - idxLastCrossingEdgeFromEndPoint + 2);
            list.Add(_arredges[idxLastCrossingEdgeFromEndPoint].First);
            for (int i = idxLastCrossingEdgeFromEndPoint; i <= idxLastCrossingEdgeFromCurrentSolution; i++)
                list.Add(_arredges[idxLastCrossingEdgeFromEndPoint].Second);
            return list;
        }

        private SolutionForDot CreateSolutionForDot(Vector2 baseDotSolution, int closestNumEdge, int farthestNumEdge, int numRecBaseDot)
        {
            //(Line notPassedLine, int numEdgeNotPassed, int closestNumEdge, Vector2 baseDotSolution) missedOneLine = default;
            List<Line> listLines = new List<Line>(2);
            foreach ((int currentTestingNumEdge, int nextEdgeAftercurrent) in GetOrderedListNumEdges(closestNumEdge, farthestNumEdge))
            {
                Debug.Log($"Trying link with Edge[{currentTestingNumEdge}]");

                foreach (Vector2 dotEdge in GitListDotsEdge(_arredges[currentTestingNumEdge]))
                {
                    //Test possibility to create line with baseDot and dot from other edge with can pass all edges between them
                    //int nextEdgeAftercurrent = currentTestingNumEdge + step;    will skip current edge fot testing pass
                    (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = TryLinkTwoDotsThroughEdges(baseDotSolution, dotEdge, closestNumEdge,
                        nextEdgeAftercurrent);
                    if (isPassedEdges)
                    {
                        listLines.Add(lineBTWBaseDotAndEdge);
                    }
                    //else
                    //{
                    //    missedOneLine = (lineBTWBaseDotAndEdge, numLastTestedEdge, closestNumEdge, baseDotSolution);
                    //}
                }
                Debug.Log($"Was found {listLines.Count()} LinkLines the current {baseDotSolution} with Edge[{currentTestingNumEdge}] ");
                switch (listLines.Count())
                {
                    case 1:
                        Debug.Log("SKIPPED: Solution HARD CASE");
                        break;
                        //listLines.Add(FindMissingLineOnEdge(missedOneLine, listLines[0]));
                        //DebugFinder.DebugTurnOn(active: true);
                        //DebugDrawLine(listLines, $"Solution HARD CASE ForEdge[{currentTestingNumEdge}]");
                        //DebugFinder.DebugTurnOn(active: false);
                        //return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)), currentTestingNumEdge);
                    case 2:
                        Debug.Log("Will create new {Solution}");
                        DebugFinder.DebugTurnOn(active: true);
                        DebugFinder.DebugDrawLine(listLines, $"Solution ForEdge[{currentTestingNumEdge}]");
                        DebugFinder.DebugTurnOn(active: false);
                        //int canBeExtendedTillEdgeNum = TryExtendNumEdgePassedByCurrentSolution(listLines, currentTestingNumEdge, farthestNumEdge);
                        //return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)),
                        //    (canBeExtendedTillEdgeNum == -1) ? currentTestingNumEdge : canBeExtendedTillEdgeNum);
                        return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)),currentTestingNumEdge, numRecBaseDot);
                    default:
                        break;
                }
                listLines.Clear();
            }
            foreach (Vector2 dotEdge in GitListDotsEdge(_arredges[closestNumEdge]))
            {
                //in case trying to link the baseDotSolution with the same edge where it exist, it will always be possible
                listLines.Add(new Line(baseDotSolution, dotEdge));
            }
            Debug.Log("Will create a new {Solution} on closest Edge");
            DebugFinder.DebugTurnOn(active: true);
            DebugFinder.DebugDrawLine(listLines, $"Solution ForEdge[{closestNumEdge}]");
            DebugFinder.DebugTurnOn(active: false);
            return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)), closestNumEdge, numRecBaseDot);
        }

        private int TryExtendNumEdgePassedByCurrentSolution(List<Line> listLines, int closestNumEdge, int farthestNumEdge)
        {
            int canBeExtendedTillEdgeNum = -1;
            foreach ((int currentTestingNumEdge, int nextEdgeAftercurrent) in GetOrderedListNumEdges(closestNumEdge, farthestNumEdge))
            {
                Debug.Log($"Trying to Extend to Edge[{currentTestingNumEdge}]");
                foreach (Line line in listLines)
                {
                    if (!line.TryIntersecLineWithEdge(currentTestingNumEdge))
                    {
                        Debug.Log($"Intersec Line Not crossing Edge[{currentTestingNumEdge}]");
                        return canBeExtendedTillEdgeNum;
                    }
                }
                canBeExtendedTillEdgeNum = currentTestingNumEdge;
            }
            return canBeExtendedTillEdgeNum;
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

        private Line FindMissingLineOnEdge((Line notPassedLine, int numEdgeNotPassed, int closestNumEdge, Vector2 baseDotSolution) missedOneLine, Line linePassed)
        {
            //Debug.Log($"notPassedLine[{missedOneLine.notPassedLine}]");
            //Debug.Log($"linePassed[{linePassed}]");
            foreach ((int currentTestingNumEdge, int nextEdgeAftercurrent) in GetOrderedListNumEdges(missedOneLine.closestNumEdge, missedOneLine.numEdgeNotPassed))
            {
                foreach (Vector2 dotEdge in GitListDotsEdge(_arredges[currentTestingNumEdge]))
                {
                    (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = TryLinkTwoDotsThroughEdges(missedOneLine.baseDotSolution, dotEdge,
                        missedOneLine.closestNumEdge, nextEdgeAftercurrent);
                    if (isPassedEdges && lineBTWBaseDotAndEdge.IsBetweenLines(linePassed, missedOneLine.notPassedLine))
                        return lineBTWBaseDotAndEdge;
                }
            }
            foreach (Vector2 dotEdge in GitListDotsEdge(_arredges[missedOneLine.closestNumEdge]))
            {
                //in case trying to link the baseDotSolution with the same edge where it exist, it will always be possible
                Line lineFromClosestEdge = new Line(missedOneLine.baseDotSolution, dotEdge);
                if (lineFromClosestEdge.IsBetweenLines(linePassed, missedOneLine.notPassedLine))
                    return lineFromClosestEdge;
            }
            throw new NotImplementedException($"Can't find line between Basedot {missedOneLine.baseDotSolution} and Egde num[{missedOneLine.closestNumEdge}]");
        }

        private IEnumerable<Vector2> GitListDotsEdge(Edge edge)
        {
            yield return edge.Start;
            yield return edge.End;
        }

        /// <summary>
        /// Try link by line two dots from certain edges
        /// </summary>
        /// <param name="dotA"></param>
        /// <param name="dotB"></param>
        /// <param name="numEdgeAfterDotA"></param>
        /// <param name="numEdgeBeforeDotB"></param>
        /// <returns> if linked (true, numberLastPassEdge) otherwise (false, numberLastNotPassEdge)</returns>
        private (bool isPassedEdge, Line line, int numLastTestedEdge) TryLinkTwoDotsThroughEdges(Vector2 dotA, Vector2 dotB, int numEdgeAfterDotA, int numEdgeBeforeDotB)
        {
            CheckEdgeNumbers(ref numEdgeAfterDotA, ref numEdgeBeforeDotB);
            Debug.Log($"TryLinkTwoDots({dotA}, {dotB}), check Edges from [{numEdgeAfterDotA}] till [{numEdgeBeforeDotB}]");
            Line lineBTWDots = new Line(dotA,dotB);
            bool directLineBTWdotsExist = true;
            int currentNumTestingEdge = numEdgeAfterDotA;
            for (; currentNumTestingEdge <= numEdgeBeforeDotB; currentNumTestingEdge++)
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

        private void CheckEdgeNumbers(ref int numEdgeAfterDotA, ref int numEdgeBeforeDotB)
        {
            if (numEdgeAfterDotA == numEdgeBeforeDotB)
                return;
             //throw new NotImplementedException($"Wrong call TryLinkTwoDotsThroughEdges() the numEdgeAfterDotA[{numEdgeAfterDotA}]==numEdgeBeforeDotB[{numEdgeBeforeDotB}]");
            if (numEdgeAfterDotA > numEdgeBeforeDotB)
            {
                int temp = numEdgeBeforeDotB;
                numEdgeBeforeDotB = numEdgeAfterDotA;
                numEdgeAfterDotA = temp;
            }
        }

        private SolutionForEdge FindCrossingCurrentSolutionWithEdge(ISolution solutionObj)
        {
            int numNewSolutions = 0;
            foreach (Solution currentSolution in solutionObj.GetListSolution())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                Vector2 baseDotCurrentSolutions = currentSolution.BaseDotIntersec;
                int numEdgeCurrentSolution = solutionObj.NumLastCrossedEdgeEdge;
                for (int numEdge = _moreClosestFromStartEdgeToRectAccessableFromEndPoint; numEdge < numEdgeCurrentSolution; numEdge--)
                {
                    Edge edge = _arredges[numEdge];
                    foreach (Vector2 basedot in GetListBaseDotFromEdge(edge))
                    {
                        foreach (Vector2 currentBaseDotSolutionin in solutionObj.GetListBasedDotsSolution())
                        {
                            Solution newSolutionForEdge = TryFindNewSolutionForEdge(currentBaseDotSolutionin, basedot, prev, out Vector2 dotCrossing);

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

        private Solution TryFindNewSolutionForEdge(Vector2 currentBaseDotSolutionin, Vector2 basedot, DotIntersec prev, out Vector2 dotCrossing)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Vector2> GetListBaseDotFromEdge(Edge edge)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Trying to find crossing of lines from current solution with lines from EndPointPath in closest Rect which accessable from EndPoint
        /// </summary>
        /// <param name="solutionObj"></param>
        /// <returns>number dots where Solution lines intersect with EndPoint in the rect</returns>
        //private int FindCrossingCurrentSolutionWithEndPoint(ISolution solutionObj)
        //{
        //    int numNewDotCorssing = 0;
            
        //    foreach (Solution currentSolution in solutionObj.GetListSolution())
        //    {
        //        DotIntersec prev = currentSolution.IntersecBaseDot;
        //        foreach (Line currentLineSolution in currentSolution.GetListLines())
        //        {
        //            foreach (Line currentLineEndPoint in _listLinesEndPoint)
        //            {
        //                if (CrossLineSolutionWithLineEndPointInRect(currentLineSolution, currentLineEndPoint, _closestRectAccessableFromEndPoint, out Vector2 dotCrossing))
        //                {
        //                    _listDotCrossing.AddDotCross(dotCrossing, prev);
        //                    numNewDotCorssing++;
        //                }
        //            }

        //        }
        //    }
        //    Debug.Log($"{this}: numNewDotCorssing[{numNewDotCorssing}]");
        //    return numNewDotCorssing;
        //}

        private void LinkCurrentSolutionBySimpleTurn(ISolution solutionObj, Edge edge)
        {
            foreach (Solution currentSolution in solutionObj.GetListSolution())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                _listDotCrossing.AddDotCross(edge.Start, new DotIntersec(edge.Start, prev));
                _listDotCrossing.AddDotCross(edge.End, new DotIntersec(edge.End, prev));
            }
        }


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

        /// <summary>
        /// Trying to find crossing of line from solution with line from EndPointPath in certain Rec
        /// </summary>
        /// <param name="currentLineSolution"></param>
        /// <param name="currentLineEndPoint"></param>
        /// <param name="closestRectAccessableFromEndPoint"></param>
        /// <param name="dotCrossing"></param>
        /// <returns>true if lines intersect and the dot of crossing in the certain rect</returns>
        private bool CrossLineSolutionWithLineEndPointInRect(Line currentLineSolution, Line currentLineEndPoint,
            Rectangle closestRectAccessableFromEndPoint, out Vector2 dotCrossing)
        {
            (Matrix2x2 matrix, VectorB2 b) = CreateDataForLinearSystemEquation(currentLineSolution, currentLineEndPoint);
            VectorB2 vector = LinearSystemEquation.GetSolutionLinearSystemEquation(matrix, b);
            if (vector == null)
            {
                Debug.LogError($"Can't find Solution for crossing line({currentLineSolution}) with line ({currentLineEndPoint})");
                dotCrossing = Vector2.zero;
                return false;
            }
            dotCrossing = (Vector2)vector;
            return StoreInfoEdges.IsDotInRect(dotCrossing, closestRectAccessableFromEndPoint);
        }

        private bool IsDotInOneFromListRect(Vector2 dot, List<Rectangle> listCheckedRec)
        {
            foreach (Rectangle rect in listCheckedRec)
            {
                if (StoreInfoEdges.IsDotInRect(dot, rect))
                    return true;
            }
            return false;
        }



        private (Matrix2x2 matrixFactors, VectorB2 b) CreateDataForLinearSystemEquation(Line currentLineSolution, Line currentLineEndPoint)
        {
            (float a11, float a12, float b1) = currentLineSolution.GetDataForMatrix2x2();
            (float a21, float a22, float b2) = currentLineEndPoint.GetDataForMatrix2x2();
            return (new Matrix2x2(a11, a12, a21, a22), new VectorB2(b1, b2));
        }
    }
}