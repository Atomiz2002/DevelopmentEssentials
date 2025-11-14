using DevelopmentEssentials.Extensions.CS;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static partial class TransformExtensions {

        public static Vector2 DirectionTowards(this Transform position, Transform targetPosition) => targetPosition.position - position.position;

        public static bool Reached(this Transform position, Transform targetPosition, float tolerance = Consts.TOLERANCE) =>
            targetPosition.Distance(position) < tolerance;

        public static float Distance(this Transform position, Transform targetPosition) => Vector2.Distance(targetPosition.position, position.position);

        public static Vector2 Forward2D(this Transform transform) {
            float angleInRadians = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            return new(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }

        /// <param name="forward">defaults to right</param>
        public static void LookAt2D(this Transform transform, Vector2 direction, Vector2Int forward = default) {
            forward.EnsureRef(Vector2Int.right);
            float angle        = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float forwardAngle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new(0, 0, angle - forwardAngle));
        }

    }

}