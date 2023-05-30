using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class ConnectionDot
    {
        public readonly Vector2 dot;
        public readonly IEnumerable<ConnectionDot> prevConnectionDots;

        public ConnectionDot(Vector2 dot, IEnumerable<ConnectionDot> prevConnectionDots)
        {
            this.dot = dot;
            this.prevConnectionDots = prevConnectionDots;
        }
    }

    public class ListDotsPath
    {
        List<ConnectionDot> _list;
        int _numDotHaveCrossingwithEndPath;
        Vector2 _endPointFindPath;
        //Intersect will be at twice more than edge
        private const int FactorIntersectToEdge = 2;
        private List<Vector2> _path;

        internal ListDotsPath(int numEdges)
        {
            _list = new List<ConnectionDot>(numEdges * FactorIntersectToEdge);
        }

        internal void AddDotCross(Vector2 rez, IEnumerable<ConnectionDot> prevConnectionDots)
        {
            _list.Add(new ConnectionDot(rez, prevConnectionDots));
        }

        private const int INCLUDESTARTANDENDPATH = 2;
        /// <summary>
        /// Return dots in Path if it exist. Order from StartPath
        /// </summary>
        /// <returns>order from start</returns>
        internal IEnumerable<Vector2> GetPath()
        {
            //Always in path include minimum two dots - startPath and endPath
            _path = new List<Vector2>(_list.Count + INCLUDESTARTANDENDPATH);
            _path.Add(_endPointFindPath);
            SelectAnyPathWithBeginLastDotCrossing();
            return _path.Reverse<Vector2>();
        }

        //Not have special optimization at selecting Dots to Path
        private void SelectAnyPathWithBeginLastDotCrossing()
        {
            Debug.LogWarning("Will take the last founded dot of crossing the endPath");
            ConnectionDot lastDotCrossing = _list[_list.Count - 1];
            //The dot of StartPath incluided in list and have .prev == null
            do
            {
                _path.Add(lastDotCrossing.dot);
                lastDotCrossing = lastDotCrossing.prevConnectionDots.ElementAt(0);
            } while (lastDotCrossing != null);
        }

        internal void SaveDataLastConnectionsWithEndPath(int numDotHaveCrossingwithEndPath, Vector2 endPointFindPath)
        {
            _endPointFindPath = endPointFindPath;
            _numDotHaveCrossingwithEndPath = numDotHaveCrossingwithEndPath;
        }
    }
}