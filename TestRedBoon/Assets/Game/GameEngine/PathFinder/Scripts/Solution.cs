using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public enum SolutionSide
    {
        Start = 0,
        End = 1,
    }
    public interface ISolution
    {
        IEnumerable<Solution> GetListSolution();
        IEnumerable<Line> GetListLinesFromSolution();
        IEnumerable<Vector2> GetListBasedDotsSolution();
        int NumLastCrossedEdgeEdge { get; }
        int NumRecBaseDotSolution { get; }
    }

    public class Solution // angleLineB > angleLineA in degrees // in k factor
    {
        public readonly Line LineB;
        public readonly Line LineA;
        public readonly DotIntersec IntersecBaseDot;
        //public readonly int NumRecBaseDot;

        public Vector2 BaseDotIntersec => IntersecBaseDot.dot;

        public Solution(List<Line> lines, DotIntersec dotCrossing)
        {
            if (lines.Count == 2)
            {
                LineB = lines[0];
                LineA = lines[1];
            }
            else
                throw new NotSupportedException($"Wrong number lines in {lines}");
            this.IntersecBaseDot = dotCrossing;
        }

        internal List<LineConnection> TryLinkSolutionWithEdgeDonCrossBordersRect(Edge edge)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Line> GetListLines()
        {
            yield return LineB;
            yield return LineA;
        }
    }
}