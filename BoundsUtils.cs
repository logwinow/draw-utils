using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiractionTeam.Utils
{
    public static class BoundsUtils
    {
        public static Rect GetRect(this Bounds bounds)
        {
            return new Rect(bounds.min, bounds.size);
        }

        public static Vector2 Top(this Bounds bounds)
        {
            return new Vector2(bounds.center.x, bounds.max.y);
        }
        
        public static Vector2 TopLeft(this Bounds bounds)
        {
            return new Vector2(bounds.min.x, bounds.max.y);
        }
        
        public static Vector2 TopRight(this Bounds bounds)
        {
            return bounds.max;
        }
        
        public static Vector2 Bottom(this Bounds bounds)
        {
            return new Vector2(bounds.center.x, bounds.min.y);
        }
        
        public static Vector2 BottomLeft(this Bounds bounds)
        {
            return bounds.min;
        }
        
        public static Vector2 BottomRight(this Bounds bounds)
        {
            return new Vector2(bounds.max.x, bounds.min.y);
        }
        
        public static Vector2 Left(this Bounds bounds)
        {
            return new Vector2(bounds.min.x, bounds.center.y);
        }
        
        public static Vector2 Right(this Bounds bounds)
        {
            return new Vector2(bounds.max.x, bounds.center.y);
        }

        public static void SetBottomLeft(ref this Bounds bounds, Vector2 position)
        {
            bounds.center = position + (Vector2)bounds.extents;
        }

        public static void SetTopLeft(this Bounds bounds, Vector2 position)
        {
            bounds.center = position + new Vector2(bounds.extents.x, -bounds.extents.y);
        }

        public static bool IsInside(this Bounds bounds, Bounds other)
        {
            return other.Contains(bounds.TopLeft()) && other.Contains(bounds.TopRight()) && other.Contains(bounds.BottomLeft()) && other.Contains(bounds.BottomRight());
        }

        public static bool IsOverlapped(this Bounds bounds, Collider2D other)
        {
            return other.OverlapPoint(bounds.TopLeft()) && other.OverlapPoint(bounds.TopRight()) && other.OverlapPoint(bounds.BottomLeft()) && other.OverlapPoint(bounds.BottomRight());
        }

        public static Bounds WorldToScreenBounds(this Bounds bounds)
        {
            var topRightPoint = Camera.main.WorldToScreenPoint(bounds.max);
            var bottomLeftPoint = Camera.main.WorldToScreenPoint(bounds.min);
            var size = topRightPoint - bottomLeftPoint;
            
            return new Bounds(size / 2 + bottomLeftPoint, size);
        }

        public static Bounds ScreenToWorldBounds(this Bounds bounds)
        {
            var topRightPoint = Camera.main.ScreenToWorldPoint(bounds.max);
            var bottomLeftPoint = Camera.main.ScreenToWorldPoint(bounds.min);
            var size = topRightPoint - bottomLeftPoint;
            
            return new Bounds(size / 2 + bottomLeftPoint, size);
        }

        public static Vector3 ScreenToWorldSize(Vector3 screenSize)
        {
            return Camera.main.ScreenToWorldPoint(screenSize);
        }

        public static Vector3 ScreenToWorldSize(this Bounds bounds)
        {
            return Camera.main.ScreenToWorldPoint(bounds.size);
        }

        public static Bounds ToBounds(this Rect rect)
        {
            return new Bounds(rect.center, rect.size);
        }
    }
}