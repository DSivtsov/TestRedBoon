using UnityEngine;

namespace GameEngine.PathFinder
{
    public static class DebugUtils
    {
        public static void DrawRect(Vector2 start, Vector2 end, Color color)
        {
            Debug.DrawLine(start, new Vector2(start.x, end.y), color);
            Debug.DrawLine(new Vector2(start.x, end.y), end, color);
            Debug.DrawLine(end, new Vector2(end.x, start.y), color);
            Debug.DrawLine(start, new Vector2(end.x, start.y), color);
        }

        public static void DrawRect(Vector2 start, Vector2 end, Color color, float duration)
        {
            Debug.DrawLine(start, new Vector2(start.x, end.y), color, duration);
            Debug.DrawLine(new Vector2(start.x, end.y), end, color, duration);
            Debug.DrawLine(end, new Vector2(end.x, start.y), color, duration);
            Debug.DrawLine(start, new Vector2(end.x, start.y), color, duration);
        }

        public static void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect.min, rect.max, color);
        }

        public static void DrawRect(Rect rect, Color color, float duration)
        {
            DrawRect(rect.min, rect.max, color, duration);
        }
    } 
}