using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static partial class VectorExtensions {

        public static Vector2 V2(this Vector3 v3)           => v3;
        public static Vector2 V2(this Vector2Int direction) => direction;

        /// Used for directions
        public static Vector2Int Int(this Vector2 direction) => Vector2Int.CeilToInt(direction.normalized);
        // public static Vector3 XY0(this Vector2 v2) => new(v2.x, v2.y, 0);

    }

}