using UnityEngine;

namespace FoW
{
    public static class FogOfWarUtils
    {
        // Returns clockwise angle [-180-180]
        public static float ClockwiseAngle(Vector2 from, Vector2 to)
        {
            float angle = Vector2.Angle(from, to);
            if (Vector2.Dot(from, new Vector2(to.y, to.x)) < 0.0f)
                angle = -angle;
            return angle;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static void SetKeywordEnabled(this Material material, string keyword, bool enable)
        {
            if (enable)
                material.EnableKeyword(keyword);
            else
                material.DisableKeyword(keyword);
        }

        public static int ManhattanMagnitude(this Vector2Int vec)
        {
            return Mathf.Abs(vec.x) + Mathf.Abs(vec.y);
        }

        public static Vector2Int ToInt(this Vector2 vec)
        {
            return new Vector2Int((int)vec.x, (int)vec.y);
        }

        public static Vector2 ToFloat(this Vector2Int vec)
        {
            return new Vector2(vec.x, vec.y);
        }
    }
}