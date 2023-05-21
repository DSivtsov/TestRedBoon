using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class DotIntersec
    {
        public readonly Vector2 dot;
        public readonly DotIntersec prev;

        public DotIntersec(Vector2 dot, DotIntersec prev)
        {
            this.dot = dot;
            this.prev = prev;
        }
    }

    public class ListIntersec
    {
        List<DotIntersec> _list;
        int _numDotHaveCrossingwithEndPath;
        Vector2 _endPointFindPath;
        //Intersect will be at twice more than edge
        private const int FactorIntersectToEdge = 2;
        private List<Vector2> _path;

        internal ListIntersec(int numEdges)
        {
            _list = new List<DotIntersec>(numEdges * FactorIntersectToEdge);
        }

        internal void AddDotCross(Vector2 rez, DotIntersec prev)
        {
            _list.Add(new DotIntersec(rez, prev));
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
            DotIntersec lastDotCrossing = _list[_list.Count - 1];
            //The dot of StartPath incluided in list and have .prev == null
            do
            {
                _path.Add(lastDotCrossing.dot);
                lastDotCrossing = lastDotCrossing.prev;
            } while (lastDotCrossing != null);
        }

        internal void SaveDataLastConnectionsWithEndPath(int numDotHaveCrossingwithEndPath, Vector2 endPointFindPath)
        {
            _endPointFindPath = endPointFindPath;
            _numDotHaveCrossingwithEndPath = numDotHaveCrossingwithEndPath;
        }
    }
}