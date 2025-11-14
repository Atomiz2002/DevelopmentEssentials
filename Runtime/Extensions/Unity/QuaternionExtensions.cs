using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class QuaternionExtensions {

        public static Quaternion LookTowardsMouse(this Vector3 position, Vector2 worldPos, ref Quaternion rotation) =>
            rotation = Quaternion.LookRotation(Vector3.forward, worldPos - position.V2());

    }

}