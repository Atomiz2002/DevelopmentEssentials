using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Extensions.Unity {

    public class ChangeGeometryType : MonoBehaviour {

        [SerializeField] internal Transform World;
        [SerializeField] internal bool      Polygons;

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChangeGeometryType))]
    public class ChangeGeometryTypeEditor : UnityEditor.Editor {

        private SerializedProperty worldProp;
        private SerializedProperty polygonsProp;

        private void OnEnable() {
            worldProp    = serializedObject.FindProperty("World");
            polygonsProp = serializedObject.FindProperty("Polygons");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(worldProp);

            if (worldProp.objectReferenceValue == null)
                return;

            EditorGUILayout.PropertyField(polygonsProp);

            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea("Modify -> Pathfinder/Save & Load/Generate Cache");
            EditorGUI.EndDisabledGroup();

            if (!GUILayout.Button("Modify"))
                return;

            ChangeGeometryType script   = (ChangeGeometryType) target;
            Transform          world    = script.World;
            bool               polygons = script.Polygons;

            foreach (CompositeCollider2D composite in world
                         .GetComponentsInChildren<CompositeCollider2D>()
                         .Where(child => !child.isTrigger))
                composite.geometryType = polygons
                    ? CompositeCollider2D.GeometryType.Polygons
                    : CompositeCollider2D.GeometryType.Outlines;
        }

    }
#endif

}