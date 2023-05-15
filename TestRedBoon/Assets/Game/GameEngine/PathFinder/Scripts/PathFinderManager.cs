using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    public class PathFinderManager : MonoBehaviour
    {
        [SerializeField] private PathFinderData _pathFinderData;
        [SerializeField] private ShowPath _showPath;
        [Space]
        [ShowInInspector] private CheckerInitialData _checkerInitialData;

        private List<Vector2> _pathFounded;
        private IPathFinder _iFinder;

        private void Awake()
        {
            _checkerInitialData = new CheckerInitialData(_pathFinderData);
        }

        [Button]
        public void CallGetPath()
        {
            if (_checkerInitialData.CheckData())
            {
                _iFinder = new Finder();
                _pathFounded = _iFinder.GetPath(_pathFinderData.StartPointFindPath, _pathFinderData.EndPointFindPath, _pathFinderData.ListEdges).ToList();
                _showPath.InitShowPath(_pathFounded);
                _showPath.Show(); 
            }
        }

        public override string ToString()
        {
            return $"StartPointFindPath{_pathFinderData.StartPointFindPath} EndPointFindPath{_pathFinderData.EndPointFindPath}" +
                $" _listEdges.Count[{_pathFinderData.ListEdges.Count}]";
        }
    }


}