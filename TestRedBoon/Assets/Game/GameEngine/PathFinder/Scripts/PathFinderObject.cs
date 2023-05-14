using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameEngine.PathFinder
{
    public class PathFinderObject : MonoBehaviour, IPathFinder
    {
        [ReadOnly, ShowInInspector] private PathFinderData _pathFinderData = new PathFinderData();

        [ReadOnly, ShowInInspector] private List<Vector2> _pathFounded;

        public PathFinderData PathFinderData => _pathFinderData;
        public List<Vector2> PathFounded => _pathFounded;

        [Button]
        public void CallGetPath()
        {
            CheckData();
            _pathFounded = GetPath(_pathFinderData._startPointFindPath, _pathFinderData._endPointFindPath, _pathFinderData._listEdges).ToList();
            ShowPath();
        }

        private void ShowPath()
        {
            if (_pathFounded != null)
            {
                if (_pathFounded.Count > 0)
                {
                    ShowPointsPath();
                }
                else
                    Debug.LogWarning("Path not Found");
            }
            else
                throw new System.NotImplementedException("GetPath not run");
        }

        private void ShowPointsPath()
        {
            for (int i = 0; i < _pathFounded.Count; i++)
            {
                Debug.Log($"[{i + 1}] {_pathFounded[i]}");
            }
        }

        [Button]
        private void CheckData()
        {
            if (_pathFinderData == null || _pathFinderData._startPointFindPath == null || _pathFinderData._endPointFindPath == null
                || _pathFinderData._listEdges == null)
            {
                throw new NotImplementedException("Initial Data not intialized");
            }
            Debug.LogWarning("CheckData()");
            if (_pathFinderData._listEdges.Count == 0 )
            {
                Debug.Log("Absent Edges");
            }
        }

        public IEnumerable<Vector2> GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges)
        {
            List<Vector2> gotPatch = new List<Vector2>();
            Debug.Log(this);
            return gotPatch;
        }

        public override string ToString()
        {
            return $"startPointFindPath{_pathFinderData._startPointFindPath} {_pathFinderData._endPointFindPath} _listEdges.Count[{_pathFinderData._listEdges.Count}]";
        }
    }

    [Serializable]
    public class PathFinderData
    {
        [ReadOnly] public Vector2 _startPointFindPath;
        [ReadOnly] public Vector2 _endPointFindPath;
        [ReadOnly] public List<Edge> _listEdges;
    }

    public struct Rectangle
    {
        public Vector2 Min;
        public Vector2 Max;
    }
    public struct Edge
    {
        public Rectangle First;
        public Rectangle Second;
        public Vector2 Start;
        public Vector2 End;
    }
    public interface IPathFinder
    {
        IEnumerable<Vector2> GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges);
    }
}