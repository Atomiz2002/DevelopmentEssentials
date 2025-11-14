using DevelopmentEssentials.Extensions.CS;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static partial class VectorExtensions {

        public static Vector2Int DirectionTowards(this Vector2 position, Vector2 targetPosition) => Vector2Int.CeilToInt((targetPosition - position).normalized);
        public static Vector2    Towards(this Vector2 position, Vector2 targetPosition)          => (targetPosition - position).normalized;

        public static bool Reached(this Vector2 position, Vector2 targetPosition, float tolerance = Consts.TOLERANCE) =>
            Vector2.Distance(position, targetPosition) <= tolerance;

        public static Vector2 FixedDeltaMovement(this Vector2 position, Vector2 target, float speed) => (position - target).normalized * (speed * Time.fixedDeltaTime);

        // public static Vector2 NormalizedDirection(Vector2 position, Vector2 targetPosition) {
        // 	return Direction(position, targetPosition).normalized;
        // }

        /// <param name="referenceAngle">Defaults to Vector2.right</param>
        /// <returns>The result of the direction angle relative to the referenced direction</returns>
        public static T BisectAngle<T>(this Vector2Int direction, T left, T right, Vector2Int referenceAngle = default) =>
            BisectAngle(direction.V2(), left, right, referenceAngle);

        /// <param name="referenceAngle">Defaults to Vector2.right</param>
        /// <returns>The result of the direction angle relative to the referenced direction</returns>
        public static T BisectAngle<T>(this Vector2 direction, T left, T right, Vector2 referenceAngle = default) =>
            Vector2.SignedAngle(direction, referenceAngle.EnsureRef(Vector2Int.right)) > 0 ? left : right;

    }

}