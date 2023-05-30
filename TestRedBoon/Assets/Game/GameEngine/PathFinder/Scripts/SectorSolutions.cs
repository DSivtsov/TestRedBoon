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
        IEnumerable<SectorSolutions> GetListSectorSolutions();
        IEnumerable<Line> GetListLinesFromSectorSolutions();
        IEnumerable<Vector2> GetListBasedDotsSolution();
        int NumLastCrossedEdgeBySolution { get; }
        int NumRecBaseDotSolution { get; }

        IEnumerable<ConnectionDot> GetListDotPathSectorSolutions();
    }

    public class SectorSolutions // angleLineB > angleLineA in degrees // in k factor
    {
        public readonly Line LineB;
        public readonly Line LineA;
        public readonly ConnectionDot connectionDot;

        //public Vector2 BaseDotSectorSolutions => connectionDots.dot;

        public SectorSolutions(List<Line> lines, ConnectionDot connectionDot)
        {
            if (lines.Count == 2)
            {
                LineB = lines[0];
                LineA = lines[1];
            }
            else
                throw new NotSupportedException($"Wrong number lines in {lines}");
            this.connectionDot = connectionDot;
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