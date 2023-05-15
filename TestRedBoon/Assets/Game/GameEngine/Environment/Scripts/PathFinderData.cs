﻿using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameEngine.Environment
{
    public struct Rectangle
    {
        public Vector2 Min;
        public Vector2 Max;

        public Rectangle(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }
    }
    public struct Edge
    {
        public Rectangle First;
        public Rectangle Second;
        public Vector2 Start;
        public Vector2 End;

        public Edge(Rectangle first, Rectangle second, Vector2 start, Vector2 end)
        {
            First = first;
            Second = second;
            Start = start;
            End = end;
        }
    }
    public interface IPathFinder
    {
        IEnumerable<Vector2> GetPath(Vector2 startPointFindPath, Vector2 endPointFindPath, IEnumerable<Edge> edges);
    }

    [Serializable]
    public class PathFinderData : MonoBehaviour
    {

        [SerializeField] private Transform _prefabInitialPoint;
        [SerializeField] private Transform _findPathStartPoint;
        [SerializeField] private Transform _findPathEndPoint;
        [SerializeField] private Transform _prefabStartEdge;
        [SerializeField] private Transform _prefabEndEdge;
        [Header("RESULT")]
        [ShowInInspector, ReadOnly] private Vector2 _startPointFindPath;
        [ShowInInspector, ReadOnly] private Vector2 _endPointFindPath;
        [ShowInInspector, ReadOnly] private List<Edge> _listEdges;

        private Transform _parentTransformAllPoint;

        public List<Edge> ListEdges => _listEdges;

        public Vector2 StartPointFindPath
        {
            get => _startPointFindPath;
            set
            {
                _startPointFindPath = value;
                SetAndActivatePoint(_findPathStartPoint, value);
            }
        }

        public Vector2 EndPointFindPath
        {
            get => _endPointFindPath;
            set
            {
                _startPointFindPath = value;
                SetAndActivatePoint(_findPathEndPoint, value);
            }
        }

        private void SetAndActivatePoint(Transform prefabPoint, Vector2 pointPosition)
        {
            Transform transform = Instantiate<Transform>(prefabPoint, _parentTransformAllPoint);
            transform.position = pointPosition;
        }

        public void Init(int minNumberEdges)
        {
            _listEdges = new List<Edge>(minNumberEdges);
            _parentTransformAllPoint = transform;
        }

        public void SetInitialPoint()
        {
            SetAndActivatePoint(_prefabInitialPoint, Vector2.zero);
        }

        [Button]
        public void DeletePoints()
        {
            foreach (Transform item in _parentTransformAllPoint)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
        public void AddEdge(NormalizedRectangle firstRect, NormalizedRectangle secondRect, Vector2 startPointOnEdge, Vector2 endPointEdge)
        {
            Rectangle first = new Rectangle(firstRect.BottomLeftAngel, firstRect.BottomLeftAngel + firstRect.SizeXY);
            Rectangle second = new Rectangle(secondRect.BottomLeftAngel, secondRect.BottomLeftAngel + firstRect.SizeXY);
            Edge edge = new Edge(first, second, startPointOnEdge, endPointEdge);
            _listEdges.Add(edge);
            CreateEdgePoints(edge);
        }

        private void CreateEdgePoints(Edge edge)
        {
            SetAndActivatePoint(_prefabStartEdge, edge.Start);
            SetAndActivatePoint(_prefabEndEdge, edge.End);
        }
    }
}