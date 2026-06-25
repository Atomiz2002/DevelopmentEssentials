using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity.Physics2D {

    public static class Layers {

        public static ContactFilter2D FilterLayerMask(int layer) =>
            new() {
                useLayerMask = true,
                layerMask    = layer
            };

        public static bool IsOnLayer(this Collider2D collider, int layer) => 1 << collider.gameObject.layer == layer;

        public static bool Touching(this Collider2D collider, int layer) => collider.GetContacts(FilterLayerMask(layer), new ContactPoint2D[1]) > 0;

    }

}