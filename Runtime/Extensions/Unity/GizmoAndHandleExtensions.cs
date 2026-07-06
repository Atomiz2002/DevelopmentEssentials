using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevelopmentEssentials.Attributes;
using DevelopmentEssentials.Extensions.CS;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public static class GizmoAndHandleExtensions {

        private const float RADIUS = .1f;

        public class GizmoDrawer : SingletonBehaviour<GizmoDrawer> {

#if UNITY_EDITOR
            private readonly Dictionary<string, Action> drawGizmos  = new();
            private readonly Dictionary<string, int>    drawnGizmos = new();

            public void Draw(Action draw) {
                // if (Event.current == null)
                    drawGizmos[new StackTrace(true).ToString()] = draw;
                // else
                    // draw.InvokeSafe();
            }

            private void OnDrawGizmos() {
                drawnGizmos.RemoveAll((x, y) => y == 10);

                foreach (KeyValuePair<string, Action> x in drawGizmos) {
                    x.Value.InvokeSafe();
                    drawnGizmos[x.Key] = drawnGizmos.TryGetValue(x.Key, out int times) ? times + 1 : 1;
                }

                drawGizmos.RemoveKeys(x => drawnGizmos.TryGetValue(x, out int times) && times == 10);
            }
#endif

        }

        public static void Draw(this Action drawGizmo) => GizmoDrawer.Instance.Draw(drawGizmo);

        #region Gizmos

        public static void DrawGizmoLine(this Vector2 start, Vector2 end, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawLine(start, end);
        }

        public static void DrawGizmoSphere(this Vector2 center, float radius = RADIUS, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawSphere(center, radius);
        }

        public static void DrawGizmoWireSphere(this Vector2 center, float radius = RADIUS, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawWireSphere(center, radius);
        }

        public static void DrawGizmoBounds(this Bounds bounds, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public static void DrawGizmoCube(this Vector2 center, Vector2 size, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawCube(center, size);
        }

        public static void DrawGizmoWireCube(this Vector2 center, Vector2 size, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawWireCube(center, size);
        }

        public static void DrawGizmoFrustum(this Matrix4x4 matrix, Color? color = null) {
            color.GizmoColor();
            Gizmos.matrix = matrix;
            Gizmos.DrawFrustum(Vector2.zero, 60f, 10f, 0.2f, 1f);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void DrawGizmoMesh(this Mesh mesh, Vector2 position, Quaternion rotation, Vector2 scale, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawMesh(mesh, position, rotation, scale);
        }

        public static void DrawGizmoRay(this Vector2 from, Vector2 direction, Color? color = null) {
            color.GizmoColor();
            Gizmos.DrawRay(from, direction);
        }

        private static void GizmoColor(this Color? color) => Gizmos.color = color ?? Color.magenta;

        #endregion

        #region Handles

        public static void DrawHandleLine(this Vector2 start, Vector2 end, Color? color = null, float thickness = 0f) {
#if UNITY_EDITOR
            color.HandlesColor();
            if (thickness > 0f)
                Handles.DrawLine(start, end, thickness);
            else
                Handles.DrawLine(start, end);
#endif
        }

        public static void DrawHandleCircle(this Vector2 center, Vector2 normal, float radius = RADIUS, Color? color = null) {
#if UNITY_EDITOR
            color.HandlesColor();
            Handles.DrawSolidDisc(center, normal, radius);
#endif
        }

        public static void DrawHandleWireDisc(this Vector2 center, Vector2 normal, float radius = RADIUS, Color? color = null, float thickness = 0f) {
#if UNITY_EDITOR
            color.HandlesColor();
            if (thickness > 0f)
                Handles.DrawWireDisc(center, normal, radius, thickness);
            else
                Handles.DrawWireDisc(center, normal, radius);
#endif
        }

        public static void DrawHandleLabel(this Vector2 position, string text, GUIStyle style = null) {
#if UNITY_EDITOR
            if (style != null)
                Handles.Label(position, text, style);
            else
                Handles.Label(position, text);
#endif
        }

        public static void DrawHandleAAConvexPolygon(this Vector3[] vertices, Color? color = null) {
#if UNITY_EDITOR
            color.HandlesColor();
            Handles.DrawAAConvexPolygon(vertices);
#endif
        }

        public static void DrawHandleBezier(this Vector2 startPosition, Vector2 endPosition, Vector2 startTangent, Vector2 endTangent, Color color, Texture2D texture, float width) {
#if UNITY_EDITOR
            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, texture, width);
#endif
        }

        public static void DrawHandleSolidArc(this Vector2 center, Vector2 normal, Vector2 from, float angle, float radius = RADIUS, Color? color = null) {
#if UNITY_EDITOR
            color.HandlesColor();
            Handles.DrawSolidArc(center, normal, from, angle, radius);
#endif
        }

        public static void DrawHandleWireArc(this Vector2 center, Vector2 normal, Vector2 from, float angle, float radius = RADIUS, Color? color = null, float thickness = 0f) {
#if UNITY_EDITOR
            color.HandlesColor();
            if (thickness > 0f)
                Handles.DrawWireArc(center, normal, from, angle, radius, thickness);
            else
                Handles.DrawWireArc(center, normal, from, angle, radius);
#endif
        }

        public static void DrawHandleCamera(this Camera camera, Color? color = null) {
#if UNITY_EDITOR
            color.HandlesColor();
            Handles.DrawCamera(Rect.MinMaxRect(0, 0, Screen.width, Screen.height), camera);
#endif
        }

        public static void DrawHandleDottedLine(this Vector2 start, Vector2 end, float screenSpaceSize, Color? color = null) {
#if UNITY_EDITOR
            color.HandlesColor();
            Handles.DrawDottedLine(start, end, screenSpaceSize);
#endif
        }

        private static void HandlesColor(this Color? color) => Handles.color = color ?? Color.magenta;

        // what the hell are these params lol
        public static void DrawCircle(this Vector2 position, float radius, object @this, string nameofRadiusField, bool solid = false) {
#if UNITY_EDITOR
            Handles.color = GetAttribute.Color(@this, nameofRadiusField);

            if (solid) Handles.DrawSolidDisc(position, Vector3.back, radius);
            else Handles.DrawWireDisc(position, Vector3.back, radius);
#endif
        }

        public static void DrawCircle(this Vector2 position, float radius = RADIUS, Color? color = null, bool solid = true, float thickness = .1f) {
#if UNITY_EDITOR
            color.HandlesColor();
            if (solid)
                Handles.DrawSolidDisc(position, Vector3.back, radius);
            else
                Handles.DrawWireDisc(position, Vector3.back, radius, thickness);

#endif
        }

        public static void DrawLine(this Vector2 start, Vector2 end, Color? color = null, float thickness = .1f) {
#if UNITY_EDITOR
            color.HandlesColor();
            Handles.DrawLine(start, end, thickness);
#endif
        }

        #endregion

    }

}