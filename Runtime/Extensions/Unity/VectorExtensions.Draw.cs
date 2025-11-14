using DevelopmentEssentials.Extensions.Attributes;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static partial class VectorExtensions {

        public static void DrawCircle(this Vector2 position, object @this, float radius, string nameofRadiusField, bool solid = false) {
#if UNITY_EDITOR
            Handles.color = Get.Color(@this, nameofRadiusField);

            if (solid) Handles.DrawSolidDisc(position, Vector3.back, radius);
            else Handles.DrawWireDisc(position, Vector3.back, radius);

#endif
        }

        public static void DrawCircle(this Vector2 position, float radius, Color color = default, bool solid = false, float thickness = .1f) {
#if UNITY_EDITOR
            if (color != default)
                Handles.color = color;

            if (solid)
                Handles.DrawSolidDisc(position, Vector3.back, radius);
            else
                Handles.DrawWireDisc(position, Vector3.back, radius, thickness);

#endif
        }

        public static void DrawLine(this Vector2 start, Vector2 end, Color color, float thickness = .1f) {
#if UNITY_EDITOR
            Handles.color = color;
            Handles.DrawLine(start, end, thickness);
#endif
        }

    }

}