using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameEngine.PathFinder
{

    public enum BasePointRectangle
    {
        TopLeft = 0,
        TopRight = 1,
        BottomRight = 2,
        BottomLeft = 3,
    }

    public class GenerateField : MonoBehaviour
    {
        [SerializeField] private FieldSettingSO _fieldSetting;

        private System.Random _random;
        private int _widthHalfField;
        private int _heightHalfField;
        //To Pass Minimum number of Edges
        private int _maxWidthRectangle;
        private int _maxHeightRectangle;

        private DrawRectangle _drawRectangle;
        private void Injection()
        {
            _drawRectangle = GetComponent<DrawRectangle>();
        }

        public void Awake()
        {
            Injection();

            _random = new System.Random(_fieldSetting.CurrentSeed);
            _widthHalfField = _fieldSetting.WidthField / 2;
            _heightHalfField = _fieldSetting.HeightField / 2;

            Debug.Log($"Field {_widthHalfField} {_heightHalfField}");
            int PosX = _fieldSetting.CenterX;
            int PosY = _fieldSetting.CenterY;
            GetMaximumHeightWidthForRectangle();
            CreateRectanagle(PosX, PosY, BasePointRectangle.TopRight);
        }

        [Button]
        private void CreateRectanagle(int PosX, int PosY, BasePointRectangle _selectedBasePoint)
        {
            Vector2 start = new Vector2(PosX, PosY);
            Debug.Log($"{start} {_selectedBasePoint}");
            for (int i = 0; i < _fieldSetting.MinNumberEdges; i++)
            {
                (Vector2 bottomLeft, Vector2 widthHeight) = GetNewNormalizedRectangle(start, _selectedBasePoint);
                Debug.Log($"bottomLeft[{bottomLeft}] widthHeight[{widthHeight}]");
                _drawRectangle.Draw(bottomLeft, widthHeight);
            }
        }

        /// <summary>
        /// Get Normalized Rectangle
        /// </summary>
        /// <returns> (Vector2 bottomLeftAngle, Vector2 topRightAngle)</returns>
        private (Vector2 bottomLeftAngel, Vector2 widthHeight) GetNewNormalizedRectangle(Vector2 initialBasePoint, BasePointRectangle currentTypeAngle)
        {
            Vector2 shiftToOtherAngleRectangel = GetShiftToOtherAngleRectangle(currentTypeAngle);
            Debug.Log($"initialBasePoint: [{currentTypeAngle}]{initialBasePoint} shift:{shiftToOtherAngleRectangel}");
            (Vector2 shiftedBottomLeftAngel, Vector2 widthHeight) = GetNormalizedRectangle(initialBasePoint, shiftToOtherAngleRectangel);
            return (initialBasePoint + shiftedBottomLeftAngel, widthHeight);
        }

        private (Vector2 shiftedBottomLeftAngel, Vector2 widthHeight) GetNormalizedRectangle(Vector2 initialBasePoint, Vector2 shift)
        {
            float shiftBottomLeftX = Math.Min(initialBasePoint.x, shift.x);
            float shiftBottomLeftY = Math.Min(initialBasePoint.y, shift.y);
            float width = Math.Max(initialBasePoint.x, shift.x) - shiftBottomLeftX;
            float height = Math.Max(initialBasePoint.y, shift.y) - shiftBottomLeftY;

            return (new Vector2(shiftBottomLeftX, shiftBottomLeftY), new Vector2(width, height));
        }

        /* 
         * Get the Shifted angle of Rectange and after that NormalizedAnglesRectangle
         * final return will give a Rec with Initial Point and TopRight point
         * this not useful for DrawRectangle.Draw() which based on widthHeight by Scaling GameObject
         * 
         * Vector2 shiftToOtherAngleRectangel = GetShiftToOtherAngleRectangle(currentTypeAngle);
         *  Debug.Log($"{initialBasePoint} {initialBasePoint + shiftToOtherAngleRectangel}");
         *   return NormalizedAnglesRectangle(initialBasePoint, initialBasePoint + shiftToOtherAngleRectangel);
         */
        /// <summary>
        /// NormalizedAngleRectangle
        /// </summary>
        /// <param name="angleRectangel1"></param>
        /// <param name="angleRectangel2"></param>
        /// <returns>(Vector2 bottomLeftAngel, Vector2 topRightAngel) </returns>
        private (Vector2 bottomLeftAngel, Vector2 topRightAngel) NormalizedAnglesRectangle(Vector2 angleRectangel1, Vector2 angleRectangel2)
        {
            float bottomLeftX = Math.Min(angleRectangel1.x, angleRectangel2.x);
            float bottomLeftY = Math.Min(angleRectangel1.y, angleRectangel2.y);
            float topRightX = Math.Max(angleRectangel1.x, angleRectangel2.x);
            float topRightY = Math.Max(angleRectangel1.y, angleRectangel2.y);

            return (new Vector2(bottomLeftX, bottomLeftY), new Vector2(topRightX, topRightY));
        }

        private Vector2 GetShiftToOtherAngleRectangle(BasePointRectangle currentTypeAngle)
        {
            int widthRectangle = _random.Next(_fieldSetting.MinWidthRectangle, (_maxWidthRectangle + 1));
            int heightRectangle = _random.Next(_fieldSetting.MinHeightRectangle, (_maxHeightRectangle + 1));
            return new Vector2(widthRectangle, heightRectangle) * GetVector2ToOtherAngleRectangle(currentTypeAngle);
        }

        private Vector2 GetVector2ToOtherAngleRectangle(BasePointRectangle basePointRectangle)
        {
            switch (basePointRectangle)
            {
                case BasePointRectangle.TopLeft:
                    return new Vector2(1, -1);
                case BasePointRectangle.TopRight:
                    return new Vector2(-1, -1);
                case BasePointRectangle.BottomRight:
                    return new Vector2(-1, 1);
                case BasePointRectangle.BottomLeft:
                    return new Vector2(1, 1);
                default:
                    throw new System.NotSupportedException($"Wrong value [{basePointRectangle}]");
            }
        }    

        /// <summary>
        /// For Instantiated New Rectangles to have a demanded number of Edges
        /// </summary>
        private void GetMaximumHeightWidthForRectangle()
        {
            _maxWidthRectangle = _widthHalfField / (_fieldSetting.MinNumberEdges + 1);
            CheckCalculatedRectangleSize(ref _maxWidthRectangle, _fieldSetting.MinWidthRectangle);
            _maxHeightRectangle = _heightHalfField / (_fieldSetting.MinNumberEdges + 1);
            CheckCalculatedRectangleSize(ref _maxHeightRectangle, _fieldSetting.MinHeightRectangle);

            Debug.Log($"_maxWidthRectangle[{_maxWidthRectangle}] _maxHeightRectangle[{_maxHeightRectangle}]");
        }

        private void CheckCalculatedRectangleSize(ref int calculatedRectangleSize, int minRectangleSize)
        {
            if (calculatedRectangleSize < minRectangleSize)
            {
                Debug.LogWarning($"CONFLICT: Calculated Rectangle Size ={_maxHeightRectangle} based on demanding number edges [{_fieldSetting.MinNumberEdges}]" +
                $" less than Set Minimum Rectangle Size ={minRectangleSize}. Calculated Rectangle Size will be overrided  by the Minimum Rectangle Size");
                calculatedRectangleSize = minRectangleSize;
            }
        }

        private BasePointRectangle GetRanomBasePointRectangle()
        {
            return (BasePointRectangle)_random.Next(0, Enum.GetValues(typeof(BasePointRectangle)).Length);
        }

        private (Vector2, Vector2) GetEdgeStartEnd(Vector2 Start)
        {
            return (new Vector2(), new Vector2());
        }
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
        IEnumerable<Vector2> GetPath(Vector2 A, Vector2 C, IEnumerable<Edge> edges);
    } 
}
