using UnityEngine;
using System;

namespace GameEngine.Environment
{
    [Serializable]
    public class NormalizedRectangle
    {
        private Vector2 _bottomLeftAngel;
        private Vector2 _sizeXY;

        private DrawRectangle _drawRectangle;
        private bool _islinkedToDrawRectangle;

        public static bool IsTurnDebugCreation { get; set; } = false;

        public Vector2 BottomLeftAngel => _bottomLeftAngel;
        public Vector2 SizeXY => _sizeXY;

        public NormalizedRectangle()
        {
            _bottomLeftAngel = Vector2.zero;
            _sizeXY = Vector2.one;
            _islinkedToDrawRectangle = false;
        }

        public NormalizedRectangle(Vector2 basePoint, Vector2 shiftToOtherAngleRectangel, DrawRectangle drawRectangle)
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
            LinkToDrawRectangle(drawRectangle);
            if (IsTurnDebugCreation) Debug.Log(this); 
        }

        private void LinkToDrawRectangle(DrawRectangle drawRectangle)
        {
            if (drawRectangle)
            {
                _drawRectangle = drawRectangle;
                _islinkedToDrawRectangle = true; 
            }
            else
                throw new NotImplementedException("NormalizedRectangl.ctor():  Not have link to DrawRectangle");

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
    }
}
