﻿using System.Collections.Generic;
using UnityEngine;
using GameEngine.Environment;
using System;

namespace GameEngine.PathFinder
{
    public class DebugPathFinder : MonoBehaviour
    {
        [SerializeField] private FieldSettingSO _fieldSetting;
        [SerializeField] private Transform _prefabLineDebug;
        [SerializeField] private Transform _prefabDotCrossDebug;

        private Transform _transforDebugFinder;
        private float _positioZTransforDebugFinder;

        private int _widthHalfField;
        private int _heightHalfField;

        private int _countLine;

        private void Awake()
        {
            _transforDebugFinder = transform;
            _positioZTransforDebugFinder = _transforDebugFinder.position.z;

            _widthHalfField = _fieldSetting.WidthField / 2;
            _heightHalfField = _fieldSetting.HeightField / 2;

            _countLine = 0;
        }

        public void ShowLine(List<Line> lines, string nameGroupLine)
        {
            string nameGroup = (nameGroupLine == null) ? $"GroupLine{_countLine++}" : nameGroupLine;
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 startDot = new Vector2(-_widthHalfField, (float)lines[i].FindYForX(-_widthHalfField));
                Vector2 endDot = new Vector2(_widthHalfField, (float)lines[i].FindYForX(_widthHalfField));
                ShowLine(startDot, endDot, $"{nameGroup}_{i}"); 
            }
        }

        public void ShowLine(Line line, string nameLine)
        {
            Vector2 startDot = new Vector2(-_widthHalfField, (float)line.FindYForX(-_widthHalfField));
            Vector2 endDot = new Vector2(_widthHalfField, (float)line.FindYForX(_widthHalfField));
            ShowLine(startDot, endDot, nameLine);
        }


        public void ShowLine(Vector2 startDot, Vector2 endDot,string nameLine)
        {
            Transform transformLine = UnityEngine.Object.Instantiate<Transform>(_prefabLineDebug, _transforDebugFinder);
            transformLine.position = new Vector3(startDot.x, startDot.y, _positioZTransforDebugFinder);
            LineRenderer lineRenderer = transformLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(1, endDot - startDot);
            transformLine.name = (nameLine == null) ? $"Line{_countLine++}" : nameLine;
        }

        public void DeleteDebugFinderLines()
        {
            foreach (Transform item in _transforDebugFinder)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }

        internal void ShowDotCross(Vector2 dot, string nameDot)
        {
            Transform transformLine = UnityEngine.Object.Instantiate<Transform>(_prefabDotCrossDebug, _transforDebugFinder);
            transformLine.position = new Vector3(dot.x, dot.y, _positioZTransforDebugFinder);
            transformLine.name = (nameDot == null) ? $"DotCross{_countLine++}" : nameDot;
        }
    }
}