using JetBrains.Annotations;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static partial class VectorExtensions {

        [Pure]
        public static Vector2Int TowardsInt(this Vector2 position, Vector2 targetPosition) =>
            Vector2Int.RoundToInt(position.Towards(targetPosition));

        [Pure]
        public static Vector2 AwayFrom(this Vector2 position, Vector2 targetPosition) =>
            (position - targetPosition).normalized;

        [Pure]
        public static Vector2 Towards(this Vector2 position, Vector2 targetPosition) =>
            (targetPosition - position).normalized;

        [Pure]
        public static bool Reached(this Vector2 position, Vector2 targetPosition, float tolerance = Consts.TOLERANCE) =>
            Vector2.Distance(position, targetPosition) <= tolerance;

        [Pure]
        public static Vector2 MoveTowards(this Vector2 position, Vector2 target, float speed) =>
            Vector2.MoveTowards(position, target, speed);

        [Pure]
        public static T BisectAngle<T>(this Vector2Int direction, T left, T right, Vector2Int referenceAngle = default) =>
            direction.V2().BisectAngle(left, right, referenceAngle.V2());

        [Pure]
        public static T BisectAngle<T>(this Vector2 direction, T left, T right, Vector2 referenceAngle = default) {
            Vector2 refDir = referenceAngle == default ? Vector2.right : referenceAngle;
            return Vector2.SignedAngle(refDir, direction) > 0 ? left : right;
        }

        [Pure]
        public static float Distance(this Vector2 a, Vector2 b) => Vector2.Distance(a, b);

    }
}