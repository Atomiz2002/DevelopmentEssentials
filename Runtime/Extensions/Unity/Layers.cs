using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class Layers {

        public const int Ground      = 1 << 3;
        public const int BoundingBox = 1 << 6;
        public const int Hitbox      = 1 << 7;
        public const int Projectiles = 1 << 8;
        public const int Decor       = 1 << 9;
        public const int Player      = 1 << 10;

        public static ContactFilter2D FilterLayerMask(int layer) =>
            new() {
                useLayerMask = true,
                layerMask    = layer
            };

        public static bool IsOnLayer(this Collider2D collider, int layer) => 1 << collider.gameObject.layer == layer;

        public static bool Touching(this Collider2D collider, int layer) => collider.GetContacts(FilterLayerMask(layer), new ContactPoint2D[1]) > 0;

    }

}