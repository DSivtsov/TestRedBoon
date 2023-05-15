using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public class PathFinderObject : MonoBehaviour, IPathFinder
    {
        [ReadOnly, ShowInInspector] private PathFinderData _pathFinderData;
        [ReadOnly, ShowInInspector] private List<Vector2> _pathFounded;

       // public PathFinderData PathFinderData => _pathFinderData;
        public List<Vector2> PathFounded => _pathFounded;

        [Button]
        public void CallGetPath()
        {
            CheckData();
            _pathFounded = GetPath(_pathFinderData.StartPointFindPath, _pathFinderData.EndPointFindPath, _pathFinderData.ListEdges).ToList();
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
            if (_pathFinderData == null || _pathFinderData.StartPointFindPath == null || _pathFinderData.EndPointFindPath == null
                || _pathFinderData.ListEdges == null)
            {
                throw new NotImplementedException("Initial Data not intialized");
            }
            Debug.LogWarning("CheckData()");
            if (_pathFinderData.ListEdges.Count == 0 )
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
            return $"startPointFindPath{_pathFinderData.StartPointFindPath} {_pathFinderData.EndPointFindPath} _listEdges.Count[{_pathFinderData.ListEdges.Count}]";
        }
    }


}