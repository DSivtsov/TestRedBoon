using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using GameEngine.PathFinder;

namespace GameEngine.Environment
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
        [SerializeField] private PathFinderObject _pathFinder;
        [SerializeField] private FieldSettingSO _fieldSetting;
        [SerializeField] private DrawRectangle _drawRectangle;
        [SerializeField] private bool _useSeedFromFieldSettingSO = true;

        private PathFinderData _pathFinderData;
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
        private int NumEdge;

        private NormalizedRectangle FirstRect;


        private void Injection()
        {
            Debug.LogWarning("Injection()");
            if (!_drawRectangle)
            {
                _drawRectangle = gameObject.GetComponent<DrawRectangle>();
                if (!_drawRectangle)
                    throw new NotImplementedException("GenerateField:  Not linked to DrawRectangle");
            }
            _pathFinderData = _pathFinder.PathFinderData;
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

        [Button]
        private void CreateField()
        {
            _widthHalfField = _fieldSetting.WidthField / 2;
            _heightHalfField = _fieldSetting.HeightField / 2;
            Debug.Log($"Field {_widthHalfField} {_heightHalfField}");
            GetMaximumHeightWidthForRectangle();

            FirstRect = CreateInitialRec();

            FirstRect.Draw();

            _pathFinderData.StartPointFindPath = GetStartPointFindPath(FirstRect);

            NumEdge = 0;
        }

        [Button]
        private NormalizedRectangle CreateInitialRec()
        {
            _isOutFromFieldLimit = false;
            _prevEdgeType = EdgeType.Nothing;
            Vector2 basePoint = new Vector2(_fieldSetting.CenterX, _fieldSetting.CenterY);

            BasePointAngleType selectedBasePointAngleType = SelectAnyAngleTypeBasePoint();

            Debug.Log($"basePoint={basePoint} AngleType[{selectedBasePointAngleType}]");
            NormalizedRectangle newNormalizedRectangle = GetNewNormalizedRectangle(basePoint, selectedBasePointAngleType);
            Debug.Log(newNormalizedRectangle);
            return newNormalizedRectangle;
        }

        private Vector2 GetStartPointFindPath(NormalizedRectangle initialRec)
        {
            EdgeType edgeTypeWhereWasStartPoint = SelectEdgeType();
            Debug.Log($"First Random edgeType[{edgeTypeWhereWasStartPoint}]");
            Vector2 RandomPointOnEdge = SelectPointOnEdge(edgeTypeWhereWasStartPoint,initialRec);
            return RandomPointOnEdge;
        }

        private Vector2 SelectPointOnEdge(EdgeType edgeTypeWhereWasStartPoint, NormalizedRectangle rec)
        {
            float randomPointOnHorizontal = rec.BottomLeftAngel.x + _random.Next(0, (int)rec.SizeXY.x);
            float randomPointOnVertical = rec.BottomLeftAngel.y + _random.Next(0, (int)rec.SizeXY.y);
            switch (edgeTypeWhereWasStartPoint)
            {
                case EdgeType.Top:
                    return new Vector2(randomPointOnHorizontal, rec.BottomLeftAngel.y + rec.SizeXY.y);

                case EdgeType.Right:
                    return new Vector2(rec.BottomLeftAngel.x + (int)rec.SizeXY.x, randomPointOnVertical);

                case EdgeType.Bottom:
                    return new Vector2(randomPointOnHorizontal, rec.BottomLeftAngel.y);

                case EdgeType.Left:
                    return new Vector2(rec.BottomLeftAngel.x, randomPointOnVertical);

                case EdgeType.Nothing:
                default:
                    throw new NotSupportedException($"Wrong value [{edgeTypeWhereWasStartPoint}]");
            }
        }



        private BasePointAngleType SelectAnyAngleTypeBasePoint()
        {
            return (BasePointAngleType)_random.Next(0, NUMEANGLE);
        }

        private EdgeType SelectEdgeType()
        {
            if (_prevEdgeType == EdgeType.Nothing)
            {
                return (EdgeType)_random.Next(0, NUMEDGES);
            }
            else
            {
                // Get list of indexes of EdgeType exclude the index _prevEdgeType and EdgeType.Nothing
                IEnumerable<int> modlistIdx = Enumerable.Range(0, NUMEDGES).Where((edge) => edge != (int)_prevEdgeType);
                // Get from this list a random position, and get corespondent value at this position and convert to EdgeType
                int rndPositionInList = _random.Next(0, modlistIdx.Count());
                int valueRNDPosition = modlistIdx.ElementAt(rndPositionInList);
                return (EdgeType)valueRNDPosition;
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
}