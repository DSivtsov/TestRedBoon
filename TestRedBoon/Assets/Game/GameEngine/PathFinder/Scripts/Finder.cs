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
        private Path _path;
        private Edge[] _arredges;
        private ListDotCrossing _listDotCrossing;
        private SolutionForDot _solutionEndPath;
        private List<Line> _listLinesEndPoint;
        private List<Vector2> _listBaseDotSolutionOfEndPoint;
        private int _idxLastCrossingEdgeFromEndPoint;
        private Rectangle _closestRectAccessableFromEndPoint;
        private int _numEdgeCurrentSolution;
        private int _moreClosestFromStartEdgeToRectAccessableFromEndPoint;

        IEnumerable<Vector2> IPathFinder.GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            _arredges = edges.ToArray();
            _path = new Path(startPointFindPath, endPointFindPath, _arredges.Length);
            Line.InitLineClass(_arredges);

            _listDotCrossing = new ListDotCrossing(_arredges.Length);
            _listDotCrossing.AddDotCross(startPointFindPath, null);

            (Line lineBTWDots, int numLastTestedEdge) = LinkTwoDotsThroughtAllEdges(startPointFindPath, endPointFindPath, 0, _arredges.Length - 1);
            if (lineBTWDots != null)
            {
                Debug.Log($"We found the direct Line from StartPath to EndPath without any turns");
                //It means that we have found a Path all dots in _listDotCrossing
                _listDotCrossing.SaveDataLastConnectionsWithEndPath(NumDotHaveCrossingwithEndPathInCaseDirectConnection, endPointFindPath);
                return _listDotCrossing.GetPath();
            }
            _solutionEndPath = CreateSolutionForDot(endPointFindPath);

            _listLinesEndPoint = _solutionEndPath.GetListLinesFromSolution().ToList();
            _idxLastCrossingEdgeFromEndPoint = _solutionEndPath.GetIdxLastCrossingEdgeFromEndPoint();
            _closestRectAccessableFromEndPoint = _arredges[_idxLastCrossingEdgeFromEndPoint].First;
            _listBaseDotSolutionOfEndPoint = _solutionEndPath.GetListEdgeDotsLastCrossingEdge();

            SolutionForEdge currentSolutionFromStartPath = new SolutionForEdge(startPointFindPath);
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
                        LinkCurrentSolutionBySimpleTurn(currentSolutionFromStartPath);
                        Debug.Log($"We found the crossing the line from PointEndPath with Last Simple Turn from currentSolution");
                        //It means that we have found a Path all dots in _listDotCrossing
                        _listDotCrossing.SaveDataLastConnectionsWithEndPath(NumDotCrossinsInCaseSimpleTurn, endPointFindPath);
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

        private SolutionForDot CreateSolutionForDot(Vector2 endPointFindPath)
        {
            bool directLineBTWdotsExist = false;
            int currentTestingNumEdge = 0;
            for (; currentTestingNumEdge <= _arredges.Length; currentTestingNumEdge++)
            {
                List <Line> listLines = new List<Line>(2);
                foreach (Vector2 dotEdge in GitListDotsEdge(_arredges[currentTestingNumEdge]))
                {
                    Line lineBTWDots = new Line(dotEdge, endPointFindPath);
                    directLineBTWdotsExist = true;
                    if (lineBTWDots.TryIntersecLineWithEdge(currentTestingNumEdge))
                    {
                        listLines.Add(lineBTWDots);
                    }
                    else
                    {
                        directLineBTWdotsExist = false;
                        break;
                    }
                }
                if (directLineBTWdotsExist)
                {
                    Solution solution = new Solution(listLines, endPointFindPath,null);
                    return new SolutionForDot(solution, currentTestingNumEdge);
                }
            }
            throw new NotImplementedException($"Can't find any solution for endPath Dot {endPointFindPath}");
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
        /// <param name="nearestNumEdgeForA"></param>
        /// <param name="nearestNumEdgeForB"></param>
        /// <returns> if linked (true, numberLastPassEdge) otherwise (false, numberLastNotPassEdge)</returns>
        private (Line line, int numLastTestingEdge) LinkTwoDotsThroughtAllEdges(Vector2 dotA, Vector2 dotB, int nearestNumEdgeForA, int nearestNumEdgeForB)
        {
            Line lineBTWDots = new Line(dotA,dotB);
            bool directLineBTWdotsExist = true;
            int currentTestingNumEdge = nearestNumEdgeForA;
            for (; currentTestingNumEdge <= nearestNumEdgeForB; currentTestingNumEdge++)
            {
                if (!lineBTWDots.TryIntersecLineWithEdge(currentTestingNumEdge))
                {
                    directLineBTWdotsExist = false;
                    break;
                }
            }
            return directLineBTWdotsExist ? (lineBTWDots, currentTestingNumEdge): (null, currentTestingNumEdge);
        }


        private SolutionForEdge FindCrossingCurrentSolutionWithEdge(SolutionForEdge solutionForEdge)
        {
            int numNewSolutions = 0;
            foreach (Solution currentSolution in ((ISolution) solutionForEdge).GetListSolution())
            {
                DotCross prev = currentSolution.DotCrossing;
                Vector2 baseDotCurrentSolutions = currentSolution.BaseDot;
                
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
                                _listDotCrossing.AddDotCross(newSolutionForEdge.BaseDot, prev);
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

        private Solution TryFindNewSolutionForEdge(Vector2 currentBaseDotSolutionin, Vector2 basedot, DotCross prev, out Vector2 dotCrossing)
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
        private int FindCrossingCurrentSolutionWithEndPoint(SolutionForEdge solutionForEdge)
        {
            int numNewDotCorssing = 0;
            
            foreach (Solution currentSolution in ((ISolution)solutionForEdge).GetListSolution())
            {
                DotCross prev = currentSolution.DotCrossing;
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

        private void LinkCurrentSolutionBySimpleTurn(SolutionForEdge solutionForEdge)
        {
            foreach (Solution currentSolution in ((ISolution)solutionForEdge).GetListSolution())
            {
                DotCross prev = currentSolution.DotCrossing;
                Vector2 baseDotCurrentSolutions = currentSolution.BaseDot;
                foreach (Vector2 dotOnEdgeFromEndPath in _listBaseDotSolutionOfEndPoint)
                {

                    _listDotCrossing.AddDotCross(dotOnEdgeFromEndPath, prev);
                }
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
    }

        public class DebugFinder
    {
        private Vector2 startPointFindPath;
        private Vector2 endPointFindPath;

        public DebugFinder(Vector2 startPointFindPath, Vector2 endPointFindPath)
        {
            this.startPointFindPath = startPointFindPath;
            this.endPointFindPath = endPointFindPath;
        }

        public void ShowLineConnections(List<LineConnection> variants)
        {
            Debug.Log($"[startPointFindPath] {startPointFindPath}");
            for (int i = 0; i < variants.Count; i++)
            {
                Debug.Log($"[{i}] {variants[i]}");
            }
            Debug.Log($"[endPointFindPath] {endPointFindPath}");
        }
    }
}