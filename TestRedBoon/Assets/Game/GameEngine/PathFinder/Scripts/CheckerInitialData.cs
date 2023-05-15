using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.Environment;


namespace GameEngine.PathFinder
{
    [Serializable]
    public class CheckerInitialData
    {
        private PathFinderData _pathFinderData;

        public CheckerInitialData(PathFinderData pathFinderData)
        {
            this._pathFinderData = pathFinderData;
        }

        [Button]
        public bool CheckData()
        {
            if (_pathFinderData == null || _pathFinderData.StartPointFindPath == null || _pathFinderData.EndPointFindPath == null
                || _pathFinderData.ListEdges == null)
            {
                Debug.LogError("Initial Data not intialized. GetPath() stoped.");
                return false;
            }

            if (_pathFinderData.ListEdges.Count == 0)
            {
                Debug.LogError("Absent Edges in Initial Data. GetPath() stoped.");
                return false;
            }

            if (CheckExistOverlapingRectangle())
            {
                Debug.LogError("Exist overlaping rectangles in Initial Data. GetPath() stoped.");
                return false;
            }

            Debug.Log($"CheckData() passed. No detected errors. Found [{_pathFinderData.ListEdges.Count}] Edges");
            return true;
        }

        /// <summary>
        /// Does exist the overlaping of Rectangles
        /// </summary>
        /// <returns>true if it exist</returns>
        private bool CheckExistOverlapingRectangle()
        {
            Debug.LogWarning("Not ExistOverlapingRectangle()");
            return false;
        }


        //public override string ToString()
        //{
        //    return $"StartPointFindPath{_pathFinderData.StartPointFindPath} EndPointFindPath{_pathFinderData.EndPointFindPath}" +
        //        $" _listEdges.Count[{_pathFinderData.ListEdges.Count}]";
        //}
    }
}