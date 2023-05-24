using System.Collections.Generic;
using UnityEngine;

namespace GameEngine.PathFinder
{
    public static class DebugFinder
    {
#if DEBUGFINDER
        private static DebugPathFinderMono _debugPathFinder;
        private static bool _initDebugPathFinder = false;
        private static bool _debugTurnOn;
#endif
        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void InitDebugFinder(bool active = true)
        {
            _debugPathFinder = UnityEngine.Object.FindObjectOfType<DebugPathFinderMono>();
            _initDebugPathFinder = true;
            _debugTurnOn = active;
            _debugPathFinder.DeleteDebugFinderLines();
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugDrawDot(Vector2 dot)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowDotCross(dot, null);
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugDrawDot(Vector2 dot, string nameDot)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowDotCross(dot, nameDot);
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugDrawLine(Line line)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(line, null);
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugDrawLine(Line line, string nameLine)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(line, nameLine);
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugDrawLineSegment(Vector2 start, Vector2 end, string nameLine)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(start, end, nameLine);
        }


        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugDrawLine(List<Line> lines, string nameGroupLine)
        {
            if (_initDebugPathFinder && _debugTurnOn)
                _debugPathFinder.ShowLine(lines, nameGroupLine);
        }

        [System.Diagnostics.Conditional("DEBUGFINDER")]
        public static void DebugTurnOn(bool active) => _debugTurnOn = active;
    }
}

