using UnityEngine;
using System;

namespace GameEngine.Environment
{
    [Serializable]
    public class NormalizedRectangle
    {
        private Vector2 _bottomLeftAngel;
        private Vector2 _sizeXY;

        private static DrawRectangle _drawRectangle;
        private static bool _islinkedToDrawRectangle;

        public static bool IsTurnDebugCreation { get; set; } = false;

        public Vector2 BottomLeftAngel => _bottomLeftAngel;
        public Vector2 SizeXY => _sizeXY;

        private static int _widthField;
        private static int _heightField;
        private static bool _isFieldLimiInited;

        public static void InitNormalizedRectangle(int widthField, int heightField, DrawRectangle drawRectangle)
        {
            LinkToDrawRectangle(drawRectangle);

            _widthField = widthField;
            _heightField = heightField;
            _isFieldLimiInited = true;
        }

        private static void LinkToDrawRectangle(DrawRectangle drawRectangle)
        {
            if (drawRectangle)
            {
                _drawRectangle = drawRectangle;
                _islinkedToDrawRectangle = true;
            }
            else
                throw new NotImplementedException("NormalizedRectangl.ctor():  Not have link to DrawRectangle");
        }

        public NormalizedRectangle()
        {
            _bottomLeftAngel = Vector2.zero;
            _sizeXY = Vector2.one;
            _islinkedToDrawRectangle = false;
        }

        public NormalizedRectangle(Vector2 basePoint, Vector2 shiftToOtherAngleRectangel)
        {
            Vector2 otherPoint = basePoint + shiftToOtherAngleRectangel;

            float bottomLeftX = Math.Min(basePoint.x, otherPoint.x);
            float bottomLeftY = Math.Min(basePoint.y, otherPoint.y);

            float topRightX = Math.Max(basePoint.x, otherPoint.x);
            float topRightY = Math.Max(basePoint.y, otherPoint.y);

            float width = topRightX - bottomLeftX;
            float height = topRightY - bottomLeftY;

            _bottomLeftAngel = new Vector2(bottomLeftX, bottomLeftY);
            _sizeXY = new Vector2(width, height);
            if (IsTurnDebugCreation) Debug.Log(this); 
        }

        public override string ToString()
        {
            return $"bottomLeftAngel[{_bottomLeftAngel}] sizeXY[{_sizeXY}]";
        }

        public void Draw()
        {
            if (_islinkedToDrawRectangle)
            {
                _drawRectangle.Draw(this); 
            }
            else
                throw new NotImplementedException("NormalizedRectangle:  Not linked to DrawRectangle");

        }

        /// <summary>
        /// Cut boundaries Rect if it was out from field Limit
        /// </summary>
        /// <param name="usedAngleType">BasePointAngleType which was used for creation secondRect</param>
        /// <returns>true if was out from FieldLimit</returns>
        public bool CutRectByFieldLimit(BasePointAngleType usedAngleType)
        {
            if (_isFieldLimiInited)
            {
                switch (usedAngleType)
                {
                    case BasePointAngleType.TopLeft:
                        Vector2 bottomRightAngle = new Vector2(_bottomLeftAngel.x + _sizeXY.x, _bottomLeftAngel.y);
                        return CheckXMax(bottomRightAngle) | CheckYMin(bottomRightAngle);

                    case BasePointAngleType.TopRight:
                        return CheckXMin(_bottomLeftAngel) | CheckYMin(_bottomLeftAngel);

                    case BasePointAngleType.BottomRight:
                        Vector2 topLeftAngle = new Vector2(_bottomLeftAngel.x, _bottomLeftAngel.y + _sizeXY.y);
                          return CheckXMin(topLeftAngle) | CheckYMax(topLeftAngle);

                    case BasePointAngleType.BottomLeft:
                        Vector2 topRightAngle = _bottomLeftAngel +  _sizeXY;
                         return CheckXMax(topRightAngle) | CheckYMax(topRightAngle);

                    default:
                        throw new NotSupportedException($"Not supported AngleType[{usedAngleType}]");
                }
            }
            throw new NotImplementedException("FieldLimit not inited");
        }

        private bool CheckYMin(Vector2 checkAngle)
        {
            if (checkAngle.y < -_heightField)
            {
                _bottomLeftAngel.y = -_heightField;
                Debug.Log("CheckYMin() = true");
                return true;
            }
            else
                return false;
        }

        private bool CheckYMax(Vector2 checkAngle)
        {
            if (checkAngle.y > _heightField)
            {
                _sizeXY.y = _heightField - _bottomLeftAngel.y;
                Debug.Log("CheckYMax() = true");
                return true;
            }
            else
                return false;
        }

        private bool CheckXMax(Vector2 checkAngle)
        {
            if (checkAngle.x > _widthField)
            {
                _sizeXY.x = _widthField - _bottomLeftAngel.x;
                Debug.Log("CheckXMax() = true");
                return true;
            }
            else
                return false;
        }

        private bool CheckXMin(Vector2 checkAngle)
        {
            if (checkAngle.x < -_widthField)
            {
                _bottomLeftAngel.x = -_widthField;
                Debug.Log("CheckXMin() = true");
                return true;
            }
            else
                return false;
        }
    }
}
