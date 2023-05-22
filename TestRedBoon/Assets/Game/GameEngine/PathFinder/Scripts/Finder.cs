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
        private ISolution _solutionEndPath;
        private List<Line> _listLinesEndPoint;
        //private List<Vector2> _listBaseDotSolutionOfEndPoint;
        private int _idxLastCrossingEdgeFromEndPoint;
        private Rectangle _closestRectAccessableFromEndPoint;
        private int _numEdgeCurrentSolution;
        private int _moreClosestFromStartEdgeToRectAccessableFromEndPoint;

        IEnumerable<Vector2> IPathFinder.GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            InitDebugFinder(active: false);
            _arredges = edges.ToArray();
            _lastNumberEdge = _arredges.Length - 1;
            Line.InitLineClass(_arredges);

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
            _solutionEndPath = CreateSolutionForDot(endPointFindPath, _lastNumberEdge, FirstNumberEdge);

            _listLinesEndPoint = _solutionEndPath.GetListLinesFromSolution().ToList();
            _idxLastCrossingEdgeFromEndPoint = _solutionEndPath.NumEdge;
            _closestRectAccessableFromEndPoint = _arredges[_idxLastCrossingEdgeFromEndPoint].First;
            Debug.Log($"SolutionForDot(endPointFindPath): _listLinesEndPoint[{_listLinesEndPoint.Count}] _idxLastCrossingEdgeFromEndPoint[{_idxLastCrossingEdgeFromEndPoint}]" +
                $" _closestRectAccessableFromEndPoint[{_closestRectAccessableFromEndPoint.Min},{_closestRectAccessableFromEndPoint.Max}]");

            Debug.LogWarning($"CreateSolutionForDot(startPointFindPath, closeEdge[{FirstNumberEdge}], farEdge[{_lastNumberEdge}])");
            ISolution currentSolutionFromStartPath = CreateSolutionForDot(startPointFindPath, FirstNumberEdge, _lastNumberEdge);
            
            int numDotHaveCrossingwithEndPoint = FindCrossingCurrentSolutionWithEndPoint(currentSolutionFromStartPath);
            if (numDotHaveCrossingwithEndPoint != 0)
            {
                Debug.Log($"We found the crossing the line from PointEndPath with Line from currentSolution");
                //It means that we have found a Path all dots in _listDotCrossing
                _listDotCrossing.SaveDataLastConnectionsWithEndPath(numDotHaveCrossingwithEndPoint, endPointFindPath);
                return _listDotCrossing.GetPath();
            }
            else
            {
                do
                {
                    _numEdgeCurrentSolution = currentSolutionFromStartPath.NumEdge;
                    _moreClosestFromStartEdgeToRectAccessableFromEndPoint = _idxLastCrossingEdgeFromEndPoint - 1;
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

        private SolutionForDot CreateSolutionForDot(Vector2 baseDotSolution, int closestNumEdge, int farthestNumEdge)
        {
            DebugTurnOn(active: true);
            //int currentTestingNumEdge;
            //int step = (closestNumEdge < farthestNumEdge) ? -1 : 1;
            //int numTestedEdge = Math.Abs(closestNumEdge - farthestNumEdge);
            (Line notPassedLine, int numEdgeNotPassed, int closestNumEdge, Vector2 baseDotSolution) missedOneLine = default;
            //for (int i = 0; i <= numTestedEdge; i++)
            //{
            //    currentTestingNumEdge = farthestNumEdge + i * step;
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
                    else
                    {
                        missedOneLine = (lineBTWBaseDotAndEdge, numLastTestedEdge, closestNumEdge, baseDotSolution);
                        Debug.Log($"Not passed numEdge[{numLastTestedEdge}]");
                    }
                }
                Debug.Log($"Was found {listLines.Count()} LinkLines the current {baseDotSolution} with Edge[{currentTestingNumEdge}] ");
                switch (listLines.Count())
                {
                    case 1:
                        Debug.Log("HARD CASE");
                        listLines.Add(FindMissingLineOnEdge(missedOneLine, listLines[0]));
                        DebugDrawLine(listLines, $"HARD CASE ForEdge[{currentTestingNumEdge}]");
                        return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)), currentTestingNumEdge);
                    case 2:
                        Debug.Log("Will Try create new {Solution}");
                        DebugDrawLine(listLines, $"Solution ForEdge[{currentTestingNumEdge}]");
                        return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)), currentTestingNumEdge);
                    default:
                        break;
                } 
            }
            foreach (Vector2 dotEdge in GitListDotsEdge(_arredges[closestNumEdge]))
            {
                //in case trying to link the baseDotSolution with the same edge where it exist, it will always be possible
                listLines.Add(new Line(baseDotSolution, dotEdge));
            }
            Debug.Log("Will create a new {Solution} on closest Edge");
            DebugDrawLine(listLines, $"Solution ForEdge[{closestNumEdge}]");
            return new SolutionForDot(new Solution(listLines, new DotIntersec(baseDotSolution, null)), closestNumEdge);
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
                    DebugDrawLineSegment(dotA, dotB, $"Not crossing Edge[{currentNumTestingEdge}]");
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

        private SolutionForEdge FindCrossingCurrentSolutionWithEdge(ISolution solutionForEdge)
        {
            int numNewSolutions = 0;
            foreach (Solution currentSolution in solutionForEdge.GetListSolution())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                Vector2 baseDotCurrentSolutions = currentSolution.BaseDotIntersec;
                
                for (int numEdge = _moreClosestFromStartEdgeToRectAccessableFromEndPoint; numEdge < _numEdgeCurrentSolution; numEdge--)
                {
                    Edge edge = _arredges[numEdge];
                    foreach (Vector2 basedot in GetListBaseDotFromEdge(edge))
                    {
                        foreach (Vector2 currentBaseDotSolutionin in solutionForEdge.GetListBasedDotsSolution())
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
        /// <param name="solutionForEdge"></param>
        /// <returns>number dots where Solution lines intersect with EndPoint in the rect</returns>
        private int FindCrossingCurrentSolutionWithEndPoint(ISolution solutionForEdge)
        {
            int numNewDotCorssing = 0;
            
            foreach (Solution currentSolution in solutionForEdge.GetListSolution())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                foreach (Line currentLineSolution in currentSolution.GetListLines())
                {
                    foreach (Line currentLineEndPoint in _listLinesEndPoint)
                    {
                        if (CrossLineSolutionWithLineEndPointInRect(currentLineSolution, currentLineEndPoint, _closestRectAccessableFromEndPoint, out Vector2 dotCrossing))
                        {
                            _listDotCrossing.AddDotCross(dotCrossing, prev);
                            numNewDotCorssing++;
                        }
                    }

                }
            }
            Debug.Log($"{this}: numNewDotCorssing[{numNewDotCorssing}]");
            return numNewDotCorssing;
        }

        private void LinkCurrentSolutionBySimpleTurn(ISolution solutionForEdge, Edge edge)
        {
            foreach (Solution currentSolution in solutionForEdge.GetListSolution())
            {
                DotIntersec prev = currentSolution.IntersecBaseDot;
                _listDotCrossing.AddDotCross(edge.Start, new DotIntersec(edge.Start, prev));
                _listDotCrossing.AddDotCross(edge.End, new DotIntersec(edge.End, prev));
            }
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
            dotCrossing =  (Vector2)LinearSystemEquation.GetSolutionLinearSystemEquation(matrix, b);
            return IsDotInRect(dotCrossing, closestRectAccessableFromEndPoint);
        }

        /// <summary>
        /// Dot into Rect or on its edges
        /// </summary>
        /// <param name="dot"></param>
        /// <param name="checkedRec"></param>
        /// <returns></returns>
        private bool IsDotInRect(Vector2 dot, Rectangle checkedRec)
        {
            return dot.x >= checkedRec.Min.x && dot.x <= checkedRec.Max.x
                && dot.y >= checkedRec.Min.y && dot.y <= checkedRec.Max.y;
        }

        private (Matrix2x2 matrixFactors, VectorB2 b) CreateDataForLinearSystemEquation(Line currentLineSolution, Line currentLineEndPoint)
        {
            (float a11, float a12, float b1) = currentLineSolution.GetDataForMatrix2x2();
            (float a21, float a22, float b2) = currentLineEndPoint.GetDataForMatrix2x2();
            return (new Matrix2x2(a11, a12, a21, a22), new VectorB2(b1, b2));
        }

        private Edge GetEdgeFromEdges(int v)
        {
            throw new NotImplementedException();
        }

#if DEBUGFINDER
        private DebugPathFinder _debugPathFinder;
        private bool _initDebugPathFinder = false;
        private bool _debugTurnOn;
#endif
        [System.Diagnostics.Conditional("DEBUGFINDER")]
        private void InitDebugFinder(bool active = true)
        {
            _debugPathFinder = UnityEngine.Object.FindObjectOfType<DebugPathFinder>();
            _initDebugPathFinder = true;
            _debugTurnOn = active;
            _debugPathFinder.DeleteDebugFinderLines();
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        private void DebugDrawLine(Line line)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(line, "");
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        private void DebugDrawLine(Line line, string nameLine)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(line, nameLine);
        }
        
        [System.Diagnostics.Conditional("DEBUGFINDER")]
        private void DebugDrawLineSegment(Vector2 start, Vector2 end, string nameLine)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(start, end, nameLine);
        }


        [System.Diagnostics.Conditional("DEBUGFINDER")]
        private void DebugDrawLine(List<Line> lines, string nameGroupLine)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(lines, nameGroupLine);
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        private void DebugTurnOn(bool active) => _debugTurnOn = active;
    }
}