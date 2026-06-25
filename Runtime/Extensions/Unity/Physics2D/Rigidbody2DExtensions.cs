using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity.Physics2D {

    public static class Rigidbody2DExtensions {

        public static void PushTo(this Rigidbody2D rb, Vector2 target, float force, float tolerance = Consts.TOLERANCE) {
            if (rb.position.Reached(target, tolerance)) return;
            rb.AddForce(rb.position.Towards(target) * force, ForceMode2D.Impulse);
        }

        public static void PushTowards(this Rigidbody2D rb, Vector2 direction, float force) =>
            rb.AddForce(direction * force, ForceMode2D.Impulse);

        public static void PushAwayFrom(this Rigidbody2D rb, Vector2 target, float force) {
            rb.AddForce((rb.position - target).normalized * force, ForceMode2D.Impulse);
        }

        public static void MoveTo(this Rigidbody2D rb, Vector2 target, float speed, float tolerance = Consts.TOLERANCE) {
            if (rb.position.Reached(target, tolerance)) {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            rb.linearVelocity = rb.position.Towards(target) * speed;
        }

        public static void MoveTowards(this Rigidbody2D rb, Vector2 target, float speed) {
            rb.linearVelocity = rb.position.Towards(target) * speed;
        }

        public static void MoveAwayFrom(this Rigidbody2D rb, Vector2 target, float speed) {
            rb.linearVelocity = (rb.position - target).normalized * speed;
        }
    }
}