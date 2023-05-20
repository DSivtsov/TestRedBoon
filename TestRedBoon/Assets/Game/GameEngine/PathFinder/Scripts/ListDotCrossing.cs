using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GameEngine.PathFinder
{
    public class DotCross
    {
        public readonly Vector2 dot;
        public readonly DotCross prev;

        public DotCross(Vector2 dot, DotCross prev)
        {
            this.dot = dot;
            this.prev = prev;
        }
    }

    public class ListDotCrossing
    {
        List<DotCross> _list;
        int _numDotHaveCrossingwithEndPath;
        Vector2 _endPointFindPath;

        internal ListDotCrossing(int numEdges)
        {
            this._list = new List<DotCross>(numEdges * 2);
        }

        internal void AddDotCross(Vector2 rez, DotCross prev)
        {
            _list.Add(new DotCross(rez, prev));
        }

        private const int INCLUDESTARTANDENDPATH = 2;
        /// <summary>
        /// Return dots in Path if it exist
        /// </summary>
        /// <returns>order from start</returns>
        internal IEnumerable<Vector2> GetPath()
        {
            //Always in path include minimum two dots - startPath and endPath
            List<Vector2> path = new List<Vector2>(_list.Count + INCLUDESTARTANDENDPATH);
            path.Add(_endPointFindPath);
            DotCross lastDotCrossing = SelectLastDotCrossing();
            //The dot of StartPath incluided in list and have .prev == null
            do
            {
                path.Add(lastDotCrossing.dot);
                lastDotCrossing = lastDotCrossing.prev;
            } while (lastDotCrossing != null);
            return path.Reverse<Vector2>();
        }

        private DotCross SelectLastDotCrossing()
        {
            Debug.LogWarning("Will take the last founded dot of crossing the endPath");
            return _list[_list.Count - 1];
        }

        internal void SaveDataLastConnectionsWithEndPath(int numDotHaveCrossingwithEndPath, Vector2 endPointFindPath)
        {
            _endPointFindPath = endPointFindPath;
            _numDotHaveCrossingwithEndPath = numDotHaveCrossingwithEndPath;
        }
    }
}