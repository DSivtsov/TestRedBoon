using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using GameEngine.Environment;
using GMTools.Math;


namespace GameEngine.PathFinder
{
    public class Finder : IPathFinder
    {
        private const int NumDotHaveCrossingwithEndPathInCaseDirectConnection = 1;
        private const int FirstNumberEdge = 0;
        private Edge[] _arredges;
        private ISolution _currentSolutionForStartPoint;
        private ISolution _solutionForEndPoint;
        private int _numRecBaseDotEnd;
        private List<Line> _listLinesSolutionEnd;
        private int _numLastCrossingEdgeFromSolutionEnd;
        private Rectangle _closestRectAccessableFromSolutionEnd;
        private Vector2 _startPointFindPath;
        private Vector2 _endPointFindPath;
        IEnumerable<Vector2> IPathFinder.GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            DebugFinder.InitDebugFinder(active: false);

            _arredges = edges.ToArray();
            _startPointFindPath = startPointFindPath;
            _endPointFindPath = endPointFindPath;
            StoreInfoEdges.InitStoreEdges(_arredges);

            ListDotsPath.InitListDotsPath(_arredges.Length);
            
            _currentSolutionForStartPoint = SolutionForDot.CreateSolutionForDot(_startPointFindPath, FirstNumberEdge, _arredges.Length - 1, SolutionSide.Start);


            _solutionForEndPoint = SolutionForDot.CreateSolutionForDot(_endPointFindPath, _arredges.Length - 1, FirstNumberEdge, SolutionSide.End);
            InitParametersSolutionEndPoint();

            IEnumerable<Vector2> path;
            do
            {
                path = TryLinkCurrentBaseDotSolutionStartWithEndPoint();
                if (path != null) return path;

                path = IsBothSolutionOnOneEdge();
                if (path != null) return path;

                path = TryCrossingCurrentSolutionWithSolutionForEndPoint();
                if (path != null) return path;

                path = TryCreateDirectLineLinkedDotsCurrentSolutionWithSolutionForEndPoint();
                if (path != null) return path;

                _currentSolutionForStartPoint = SolutionForEdgeForStartPoint.CreateNewSolutionForEdge(_currentSolutionForStartPoint, _arredges.Length - 1);

            } while (false);
            return null;
        }

        /// <summary>
        /// Check that the Both Solution have dots on one Edge
        /// </summary>
        /// <returns>if Both Solution have dots on one Edge return path, in other case return null</returns>
        private IEnumerable<Vector2> IsBothSolutionOnOneEdge()
        {
            if (_numLastCrossingEdgeFromSolutionEnd == _currentSolutionForStartPoint.NumLastCrossedEdgeBySolution)
            {
                Debug.LogWarning($"We found the Both Solution have dots on one Edge");
                //Both Solution have dots on one Edge, the can be SolutionForDot (2 DotCross) and SolutionForEdfe (4 DotCross)
                int numDotHaveCrossingwithEndPoint = BothSolutionHaveDotsOnOneEdge();
                ListDotsPath.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, _endPointFindPath);
                return ListDotsPath.GetPath();
            }
            return null;
        }

        /// <summary>
        /// Try find Link between current BaseDotSolutionStart with EndPoint
        /// </summary>
        /// <returns>if Link exist return path, in other case return null</returns>
        private IEnumerable<Vector2> TryLinkCurrentBaseDotSolutionStartWithEndPoint()
        {
            int numLastCrossingEdgeFromSolutionStart = _currentSolutionForStartPoint.NumLastCrossedEdgeBySolution;
            foreach (Vector2 baseDotCurrentStartSolution in _currentSolutionForStartPoint.GetListBasedDotsSolution())
            {
                (bool isPassedEdges, _, _) = Line.TryLinkTwoDotsThroughEdges(baseDotCurrentStartSolution, _endPointFindPath,
                    numLastCrossingEdgeFromSolutionStart, _numLastCrossingEdgeFromSolutionEnd);
                if (isPassedEdges)
                {
                    Debug.LogWarning($"We found the direct Line from StartPath to EndPath without any turns");
                    ListDotsPath.SaveDataLastConnectionsWithEndPath(NumDotHaveCrossingwithEndPathInCaseDirectConnection, _endPointFindPath);
                    return ListDotsPath.GetPath();
                }
            }
            return null;
        }

        private void InitParametersSolutionEndPoint()
        {
            _numRecBaseDotEnd = _solutionForEndPoint.NumRecBaseDotSolution;
            _listLinesSolutionEnd = _solutionForEndPoint.GetListLinesFromSectorSolutions().ToList();
            _numLastCrossingEdgeFromSolutionEnd = _solutionForEndPoint.NumLastCrossedEdgeBySolution;
            _closestRectAccessableFromSolutionEnd = _arredges[_numLastCrossingEdgeFromSolutionEnd].First;
            Debug.LogWarning($"SolutionForDot(endPointFindPath): _listLinesEndPoint[{_listLinesSolutionEnd.Count}]" +
                $" _idxLastCrossingEdgeFromEndPoint[{_numLastCrossingEdgeFromSolutionEnd}]" +
                $" _closestRectAccessableFromEndPoint[{_closestRectAccessableFromSolutionEnd.Min},{_closestRectAccessableFromSolutionEnd.Max}]");
        }

        /// <summary>
        /// Try create direct Line between dots of current SolutionStart with dots of SolutionForEndPoint
        /// </summary>
        /// <returns>if the lines created return path, in other case return null</returns>
        private IEnumerable<Vector2> TryCreateDirectLineLinkedDotsCurrentSolutionWithSolutionForEndPoint()
        {
            Debug.LogWarning("CreateDirectLineLinkedCurrentSolutionWithSolutionForEndPoint(currentSolutionFromStartPath)");
            int numDotHaveCrossingwithEndPoint = 0;
            List<Line> listLines = new List<Line>(8);
            int numEdgeCurrentSolutionStart = _currentSolutionForStartPoint.NumLastCrossedEdgeBySolution;
            foreach (SectorSolutions currentSectorSolutionsStart in _currentSolutionForStartPoint.GetListSectorSolutions())
            {
                IEnumerable<ConnectionDot> prevConnectionDots = _currentSolutionForStartPoint.GetListConnectionDotsSolution();
                Debug.Log($"Trying link with Edge[{numEdgeCurrentSolutionStart}]");

                foreach (Vector2 dotEdgeStart in StoreInfoEdges.GetListDotsEdge(numEdgeCurrentSolutionStart))
                {
                    foreach (var dotEdgeEnd in StoreInfoEdges.GetListDotsEdge(_numLastCrossingEdgeFromSolutionEnd))
                    {
                        (bool isPassedEdges, Line lineBTWBaseDotAndEdge, int numLastTestedEdge) = Line.TryLinkTwoDotsThroughEdges(dotEdgeStart, dotEdgeEnd, numEdgeCurrentSolutionStart,
                            _numLastCrossingEdgeFromSolutionEnd);
                        if (isPassedEdges)
                        {
                            listLines.Add(lineBTWBaseDotAndEdge);
                            ListDotsPath.AddConnectionDot(new ConnectionDot( dotEdgeStart, prevConnectionDots));
                            numDotHaveCrossingwithEndPoint ++;
                        } 
                    }
                }

            }
            if (numDotHaveCrossingwithEndPoint != 0)
            {
                Debug.LogWarning($"We found [{numDotHaveCrossingwithEndPoint}] direct lines between the edge of currentSolution and the solution for PointEndPath");
                //It means that we have found a direct lines between the edge of currentSolution and the solution for PointEndPath
                ListDotsPath.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, _endPointFindPath);
                return ListDotsPath.GetPath();
            }
            return null;
        }

        /// <summary>
        /// Try find crossing of Lines the SolutionStart with SolutionEndPoint
        /// </summary>
        /// <returns>if crossing exist return path, in other case return null</returns>
        private IEnumerable<Vector2> TryCrossingCurrentSolutionWithSolutionForEndPoint()
        {
            Debug.LogWarning("FindCrossingCurrentSolutionWithSolutionForEndPoint(currentSolutionFromStartPath)");
            int numDotHaveCrossingwithEndPoint = 0;
            DebugFinder.DebugTurnOn(true);
            int numLastCrossingEdgeFromStart = _currentSolutionForStartPoint.NumLastCrossedEdgeBySolution;
            int numRecBaseDotStart = _currentSolutionForStartPoint.NumRecBaseDotSolution;
            //Debug.Log($"idxLastCrossingEdgeFromCurrentSolution[{numLastCrossingEdgeFromStart}] idxLastCrossingEdgeFromEndPoint[{_numLastCrossingEdgeFromSolutionEnd}]");
            foreach (SectorSolutions currentSolutionStart in _currentSolutionForStartPoint.GetListSectorSolutions())
            {
                Vector2 baseDotStart = currentSolutionStart.baseDotSectorSolutions;

                IEnumerable<ConnectionDot> prevConnectionDots = _currentSolutionForStartPoint.GetListConnectionDotsSolution();
                foreach (Line lineSolutionStart in currentSolutionStart.GetListLines())
                {
                    foreach (Line lineSolutionEnd in _listLinesSolutionEnd)
                    {
                        (bool isCrossing, Vector2 dotCrossing) = CrossLineSolutionWithLine(lineSolutionStart, lineSolutionEnd);
                        if (isCrossing)
                        {
                            if (StoreInfoEdges.IsDotOnEdge(dotCrossing, numLastCrossingEdgeFromStart))
                            {
                                //Debug.Log("Exist DotCrossing On LastCrossingEdge FromCurrentSolution");
                                AddDotCrossing(dotCrossing, prevConnectionDots, ref numDotHaveCrossingwithEndPoint);
                            }
                            else if (StoreInfoEdges.IsDotOnEdge(dotCrossing, _numLastCrossingEdgeFromSolutionEnd))
                            {
                                //Debug.Log("Exist DotCrossing On _numLastCrossingEdge FromSolutionEnd");
                                AddDotCrossing(dotCrossing, prevConnectionDots, ref numDotHaveCrossingwithEndPoint);
                            }
                            else if (StoreInfoEdges.IsDotCrossingBetweenBaseDotAndEdge(dotCrossing, baseDotStart, numLastCrossingEdgeFromStart))
                            {//DotCrossing Between BaseDotStart And LastCrossingEdgeFromCurrentSolution
                                //Debug.Log("Check Existed DotCrossing Between BaseDotStart And LastCrossingEdgeFromCurrentSolution");
                                (bool dotInRec, int numRect) = StoreInfoEdges.IsDotInRectBetweenRecBaseDotAndRectEdge(dotCrossing, numRecBaseDotStart, numLastCrossingEdgeFromStart,
                                    SolutionSide.Start);
                                if (dotInRec && IsLinePassEdges(lineSolutionEnd, _numLastCrossingEdgeFromSolutionEnd, numRect, SolutionSide.Start))
                                    AddDotCrossing(dotCrossing, prevConnectionDots, ref numDotHaveCrossingwithEndPoint);
                            }
                            else if (StoreInfoEdges.IsDotCrossingBetweenBaseDotAndEdge(dotCrossing, _endPointFindPath, _numLastCrossingEdgeFromSolutionEnd))
                            {//DotCrossing Between _numLastCrossingEdgeFromSolutionEnd and BaseDotEnd
                                //Debug.Log("Check Existed DotCrossing Between _numLastCrossingEdgeFromSolutionEnd and BaseDotEnd");
                                (bool dotInRec, int numRect) = StoreInfoEdges.IsDotInRectBetweenRecBaseDotAndRectEdge(dotCrossing, _numRecBaseDotEnd,
                                    _numLastCrossingEdgeFromSolutionEnd, SolutionSide.End);
                                if (dotInRec && IsLinePassEdges(lineSolutionStart, numLastCrossingEdgeFromStart, numRect, SolutionSide.End))
                                    AddDotCrossing(dotCrossing, prevConnectionDots, ref numDotHaveCrossingwithEndPoint);
                            }
                            else
                            {//Dot cross Between edges Solution Start and End
                                //Debug.Log("Check Existed Dot cross Between edges Solution Start and End");
                                (bool dotInRec, int numRect) = StoreInfoEdges.IsDotInRectBetweenEdges(dotCrossing, numLastCrossingEdgeFromStart, _numLastCrossingEdgeFromSolutionEnd);
                                if (dotInRec)
                                {
                                    //Debug.Log($"DotInRect[{numRect}]");
                                    bool rezLineSolutionStart = IsLinePassEdges(lineSolutionStart, numLastCrossingEdgeFromStart, numRect, SolutionSide.End);
                                    bool rezLineSolutionEnd = IsLinePassEdges(lineSolutionEnd, _numLastCrossingEdgeFromSolutionEnd, numRect, SolutionSide.Start);
                                    if (rezLineSolutionStart && rezLineSolutionEnd)
                                        AddDotCrossing(dotCrossing, prevConnectionDots, ref numDotHaveCrossingwithEndPoint);
                                }
                            }
                        }
                    }

                }
            }
            DebugFinder.DebugTurnOn(false);
            Debug.Log($"{this}: numNewDotCorssing[{numDotHaveCrossingwithEndPoint}]");
            if (numDotHaveCrossingwithEndPoint != 0)
            {
                Debug.LogWarning($"We found the crossing the line from PointEndPath with Line from currentSolution");
                //It means that we have found a Path all dots in _listDotCrossing
                ListDotsPath.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, _endPointFindPath);
                return ListDotsPath.GetPath();
            }
            return null;
        }

        private void AddDotCrossing(Vector2 dotCrossing, IEnumerable<ConnectionDot> prevConnectionDots, ref int numDotHaveCrossingwithEndPoint)
        {
            DebugFinder.DebugDrawDot(dotCrossing);
            ListDotsPath.AddConnectionDot(new ConnectionDot(dotCrossing, prevConnectionDots));
            numDotHaveCrossingwithEndPoint++;
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

        private int BothSolutionHaveDotsOnOneEdge()
        {
            int numDotHaveCrossingwithEndPoint = 0;
            Edge edge = _arredges[_currentSolutionForStartPoint.NumLastCrossedEdgeBySolution];
            foreach (SectorSolutions currentSolution in _currentSolutionForStartPoint.GetListSectorSolutions())
            {
                IEnumerable<ConnectionDot> prevConnectionDots = _currentSolutionForStartPoint.GetListConnectionDotsSolution();
                ListDotsPath.AddConnectionDot(new ConnectionDot(edge.Start, prevConnectionDots));
                ListDotsPath.AddConnectionDot(new ConnectionDot(edge.End, prevConnectionDots));
                numDotHaveCrossingwithEndPoint += 2;
            }
            return numDotHaveCrossingwithEndPoint;
        }

        private static bool IsLinePassedThroughEdges(Line line, int numEdgeStart, int numEdgeEnd)
        {
            bool directLineBTWdotsExist = true;
            //Debug.Log($"IsLinePassedThroughEdges() numEdgeStart[{numEdgeStart}] numEdgeEnd[{numEdgeEnd}]");
            for (int currentNumTestingEdge = numEdgeStart; currentNumTestingEdge <= numEdgeEnd; currentNumTestingEdge++)
            {
                if (!line.TryIntersecLineWithEdge(currentNumTestingEdge))
                {
                    //Debug.Log($"Intersec Line Not crossing Edge[{currentNumTestingEdge}]");
                    directLineBTWdotsExist = false;
                    break;
                }
            }
            return directLineBTWdotsExist;
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

        private (Matrix2x2 matrixFactors, VectorB2 b) CreateDataForLinearSystemEquation(Line currentLineSolution, Line currentLineEndPoint)
        {
            (float a11, float a12, float b1) = currentLineSolution.GetDataForMatrix2x2();
            (float a21, float a22, float b2) = currentLineEndPoint.GetDataForMatrix2x2();
            return (new Matrix2x2(a11, a12, a21, a22), new VectorB2(b1, b2));
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
    }
}