using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.Environment;

namespace GameEngine.PathFinder
{
    public class ShowPath : MonoBehaviour
    {
        [ReadOnly, ShowInInspector] private List<Vector2> _pathFounded;

        public void InitShowPath(List<Vector2> pathFounded)
        {
            _pathFounded = pathFounded;
        }

        public List<Vector2> PathFounded => _pathFounded;

        [Button]
        public void Show()
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
    } 
}
