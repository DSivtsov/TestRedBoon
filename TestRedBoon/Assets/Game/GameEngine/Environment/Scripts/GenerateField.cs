using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameEngine.PathFinder
{
    public enum BasePointAngleType
    {
        TopLeft = 0,
        TopRight = 1,
        BottomRight = 2,
        BottomLeft = 3,
    }

    public enum EdgeType
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
        Nothing = 4,
    }

    public class GenerateField : MonoBehaviour
    {
        [SerializeField] private FieldSettingSO _fieldSetting;
        [SerializeField] private DrawRectangle _drawRectangle;
        [SerializeField] private bool _useSeedFromFieldSettingSO = true;

        private System.Random _random;
        private int _widthHalfField;
        private int _heightHalfField;
        //To Pass Minimum number of Edges
        private int _maxWidthRectangle;
        private int _maxHeightRectangle;
        private EdgeType _prevEdgeType;
        private bool _isOutFromFieldLimit;
        private const int NUMEDGES = 4;
        private const int NUMEANGLE = 4;

        private void Injection()
        {
            Debug.LogWarning("Injection()");
            if (!_drawRectangle)
            {
                _drawRectangle = gameObject.GetComponent<DrawRectangle>();
                if (!_drawRectangle)
                    throw new NotImplementedException("GenerateField:  Not linked to DrawRectangle");
            }
        }

        public void Awake()
        {
            Injection();
            InitRandom();
            CreateField();
        }

        private void InitRandom()
        {
            if (_useSeedFromFieldSettingSO)
                _random = new System.Random(_fieldSetting.CurrentSeed);
            else
                _random = new System.Random();
        }

        private void CreateField()
        {
            _widthHalfField = _fieldSetting.WidthField / 2;
            _heightHalfField = _fieldSetting.HeightField / 2;
            Debug.Log($"Field {_widthHalfField} {_heightHalfField}");
            GetMaximumHeightWidthForRectangle();

            CreateInitialRec();
        }

        [Button]
        private void CreateInitialRec()
        {
            _isOutFromFieldLimit = false;
            _prevEdgeType =  EdgeType.Nothing;
            Vector2 basePoint = new Vector2(_fieldSetting.CenterX, _fieldSetting.CenterY);

            BasePointAngleType selectedBasePointAngleType = SelectAnyAngleTypeBasePoint();

            Debug.Log($"basePoint={basePoint} AngleType[{selectedBasePointAngleType}]");
            NormalizedRectangle newNormalizedRectangle = GetNewNormalizedRectangle(basePoint, selectedBasePointAngleType);

            Debug.Log(newNormalizedRectangle);
            newNormalizedRectangle.Draw();
        }

        private BasePointAngleType SelectAnyAngleTypeBasePoint()
        {
            return (BasePointAngleType)_random.Next(0, NUMEANGLE);
        }

        private EdgeType SelectEdgeTypeForNewRect()
        {
            if (_prevEdgeType != EdgeType.Nothing)
            {
                return (EdgeType)_random.Next(0, NUMEDGES);
            }
            else
            {
                EdgeType randomEdgeType = (EdgeType)_random.Next(0, NUMEDGES);
                return(EdgeType)_random.Next(0, NUMEDGES);
            }
        }

        /// <summary>
        /// Get Normalized Rectangle
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="selectedBasePointAngleType"></param>
        /// <returns></returns>
        private NormalizedRectangle GetNewNormalizedRectangle(Vector2 basePoint, BasePointAngleType selectedBasePointAngleType)
        {
            Vector2 shiftToOtherAngleRectangel = GetShiftToOtherAngleRectangle(selectedBasePointAngleType);
            Debug.Log($"initialBasePoint: [{selectedBasePointAngleType}]{basePoint} shift:{shiftToOtherAngleRectangel}");
            NormalizedRectangle newNormalizedRectangle = new NormalizedRectangle(basePoint, shiftToOtherAngleRectangel,_drawRectangle);
            return newNormalizedRectangle;
        }

        private Vector2 GetShiftToOtherAngleRectangle(BasePointAngleType basePointAngleType)
        {
            int widthRectangle = _random.Next(_fieldSetting.MinWidthRectangle, (_maxWidthRectangle + 1));
            int heightRectangle = _random.Next(_fieldSetting.MinHeightRectangle, (_maxHeightRectangle + 1));
            return new Vector2(widthRectangle, heightRectangle) * GetDirectionShiftToOtherAngleRectangle(basePointAngleType);
        }

        private Vector2 GetDirectionShiftToOtherAngleRectangle(BasePointAngleType basePointAngleType)
        {
            switch (basePointAngleType)
            {
                case BasePointAngleType.TopLeft:
                    return new Vector2(1, -1);
                case BasePointAngleType.TopRight:
                    return new Vector2(-1, -1);
                case BasePointAngleType.BottomRight:
                    return new Vector2(-1, 1);
                case BasePointAngleType.BottomLeft:
                    return new Vector2(1, 1);
                default:
                    throw new System.NotSupportedException($"Wrong value [{basePointAngleType}]");
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
