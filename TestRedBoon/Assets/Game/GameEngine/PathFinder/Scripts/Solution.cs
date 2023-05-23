using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public interface ISolution
    {
        public IEnumerable<Solution> GetListSolution();
        public IEnumerable<Line> GetListLinesFromSolution();
        public IEnumerable<Vector2> GetListBasedDotsSolution();
        public int NumEdge { get; }
    }

    public class Solution // angleLineB > angleLineA in degrees // in k factor
    {
        public readonly Line LineB;
        public readonly Line LineA;
        public readonly DotIntersec IntersecBaseDot;

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