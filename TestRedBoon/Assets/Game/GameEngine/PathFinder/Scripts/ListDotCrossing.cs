using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class ConnectionDot
    {
        public readonly Vector2 baseDot;
        public readonly IEnumerable<ConnectionDot> prevConnectionDots;

        public ConnectionDot(Vector2 connectionDot, IEnumerable<ConnectionDot> prevConnectionDots)
        {
            this.baseDot = connectionDot;
            this.prevConnectionDots = prevConnectionDots;
        }

        public override string ToString()
        {
            //Debug.Log($"connectionDot{connectionDot} prevConnectionDots!=null[{prevConnectionDots != null}] Count[{prevConnectionDots?.Count()}]");
            IEnumerable<string> values = prevConnectionDots
                .Select<ConnectionDot, string>((connectionDots) => (connectionDots == null) ? "NULL":connectionDots.baseDot.ToString()) ;
            var listDots = string.Join(" ", values);
            return $"connectionDot{baseDot} prevConnectionDots.Count[{prevConnectionDots.Count()}] listDots[{listDots}]";
        }
    }

    public static class ListDotsPath
    {
        //Intersect will be at twice more than edge
        private const int FactorIntersectToEdge = 2;

        private static List<ConnectionDot> _list;
        private static int _numDotHaveCrossingwithEndPath;
        private static Vector2 _endPointFindPath;
        private static List<Vector2> _path;

        internal static void InitListDotsPath(int numEdges)
        {
            _list = new List<ConnectionDot>(numEdges * FactorIntersectToEdge);
        }

        internal static void AddConnectionDot(ConnectionDot connectionDot)
        {
            _list.Add(connectionDot);
        }

        private const int INCLUDESTARTANDENDPATH = 2;
        /// <summary>
        /// Return dots in Path if it exist. Order from StartPath
        /// </summary>
        /// <returns>order from start</returns>
        internal static IEnumerable<Vector2> GetPath()
        {
            //Always in path include minimum two dots - startPath and endPath
            _path = new List<Vector2>(_list.Count + INCLUDESTARTANDENDPATH);
            for (int i = 0; i < _list.Count; i++)
            {
                Debug.Log($"[{i}] {_list[i]}");
            }
            SelectAnyPathWithBeginLastDotCrossing();
            return _path.Reverse<Vector2>();
        }

        //Not have special optimization at selecting Dots to Path
        private static void SelectAnyPathWithBeginLastDotCrossing()
        {
            //add the EndDotPath
            _path.Add(_endPointFindPath);
            Debug.LogWarning("Will build the path through the lastDotCrossing");
            ConnectionDot lastDotCrossing = _list[_list.Count - 1];
            //The dot of StartPath incluided in list and have .prev == null
            do
            {
                _path.Add(lastDotCrossing.baseDot);
                IEnumerable<ConnectionDot> colectionPreviousConnectionDots = lastDotCrossing.prevConnectionDots;
                lastDotCrossing = (colectionPreviousConnectionDots.Count() == 0) ? null : colectionPreviousConnectionDots.ElementAt(0);
            } while (lastDotCrossing != null);
            //add the StartDotPath
            //_path.Add(lastDotCrossing.connectionDot);

        }

        internal static void SaveDataLastConnectionsWithEndPath(int numDotHaveCrossingwithEndPath, Vector2 endPointFindPath)
        {
            _endPointFindPath = endPointFindPath;
            _numDotHaveCrossingwithEndPath = numDotHaveCrossingwithEndPath;
        }
    }
}