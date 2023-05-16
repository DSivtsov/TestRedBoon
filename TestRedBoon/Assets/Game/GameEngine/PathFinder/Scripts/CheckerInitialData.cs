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
            Rectangle[] allRectangles = GetListAllRectangles(_pathFinderData.ListEdges);
            for (int idxCheckedRec = 0; idxCheckedRec < allRectangles.Length - 1; idxCheckedRec++)
            {
                Rectangle checkedRec = allRectangles[idxCheckedRec];
                for (int idxOtherRec = idxCheckedRec + 1; idxOtherRec < allRectangles.Length; idxOtherRec++)
                {
                    if (ExistOverlapingRectangle(checkedRec, allRectangles[idxOtherRec]))
                    {
                        Debug.Log($"idxCheckedRec[{idxCheckedRec}] OtherRect[{idxOtherRec}] ExistOverlapingRectangle[true]");
                        return true;
                    }
                }
            }
            return false;
        }



        private bool ExistOverlapingRectangle(Rectangle checkedRec, Rectangle otherRec)
        {
            Vector2[] angles = GetAnglesFromRectangle(otherRec);
            for (int angle = 0; angle < angles.Length; angle++)
            {
                if (IsAnglesInRect(angles[angle], checkedRec))
                {
                    Debug.Log($"DEMO!!! AngleType[{(AngleType)angle}] IsAnglesInRect[true]");
                    Debug.Log($"checkedRec[{show(checkedRec)}] angleOtherRec{angles[angle]} ");
                    return true; 
                }
            }
            return false;
        }

        private string show(Rectangle rec) => $"Min{rec.Min} Max{rec.Max}";

        private bool IsAnglesInRect(Vector2 angle, Rectangle checkedRec)
        {
            return angle.x > checkedRec.Min.x && angle.x < checkedRec.Max.x
                && angle.y > checkedRec.Min.y && angle.y < checkedRec.Max.y;
        }

        private Vector2[] GetAnglesFromRectangle(Rectangle otherRec)
        {
            return new Vector2[]
            {
                new Vector2(otherRec.Min.x,otherRec.Max.y), //TopLeft = 0,
                new Vector2(otherRec.Max.x,otherRec.Max.y), //TopRight = 1,
                new Vector2(otherRec.Max.x,otherRec.Min.y), //BottomRight = 2,
                new Vector2(otherRec.Min.x,otherRec.Min.y), //BottomLeft = 3,
            };
        }

        private Rectangle[] GetListAllRectangles(List<Edge> listEdges)
        {
            Rectangle[] allRectangles = new Rectangle[listEdges.Count + 1];
            allRectangles[0] = listEdges[0].First;
            for (int i = 0; i < listEdges.Count; i++)
            {
                allRectangles[i + 1] = listEdges[i].Second;
            }
            return allRectangles;
        }


        //public override string ToString()
        //{
        //    return $"StartPointFindPath{_pathFinderData.StartPointFindPath} EndPointFindPath{_pathFinderData.EndPointFindPath}" +
        //        $" _listEdges.Count[{_pathFinderData.ListEdges.Count}]";
        //}
    }
}