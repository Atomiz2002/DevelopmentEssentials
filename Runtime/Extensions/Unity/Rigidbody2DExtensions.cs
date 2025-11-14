using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class Rigidbody2DExtensions {

        public static void MoveTowards(this Rigidbody2D rb, Vector2 target, float speed, float tolerance = Consts.TOLERANCE) {
            Vector2 position = rb.position;

            if (position.Reached(target, tolerance))
                return;

            rb.MovePosition(position - position.FixedDeltaMovement(target, speed));
        }

        public static void MoveAwayFrom(this Rigidbody2D rb, Vector2 target, float speed, float tolerance = Consts.TOLERANCE) {
            Vector2 position = rb.position;
            if (Vector2.Distance(position, target) < tolerance) return;

            rb.MovePosition(position + position.FixedDeltaMovement(target, speed));
        }

    }

}