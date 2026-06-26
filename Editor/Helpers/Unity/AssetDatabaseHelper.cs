using System;
using System.Collections.Generic;
using DevelopmentEssentials.Editor.Extensions.Unity;
using DevelopmentEssentials.Extensions.CS;
using DevelopmentEssentials.Extensions.Unity;
using JetBrains.Annotations;
using UnityEditor;
using Object = UnityEngine.Object;

namespace DevelopmentEssentials.Editor.Helpers.Unity {

    public static class AssetDatabaseHelper {

        [Pure]
        public static List<string> GetSelectedGUIDsRecursively<T>() => GetSelectedGUIDsRecursively("t:" + typeof(T).Name);

        [Pure]
        public static List<string> GetSelectedGUIDsRecursively(string filter = "") {
            List<string> selectedGUIDs = new(Selection.assetGUIDs);

            if (selectedGUIDs.Count == 0)
                return new();

            foreach (string guid in selectedGUIDs.ToArray()) {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (AssetDatabase.IsValidFolder(path)) // If it's a folder, get all texture GUIDs inside it
                    selectedGUIDs.AddRange(AssetDatabase.FindAssets(filter, new[] { path }));
            }

            return selectedGUIDs;
        }

        public static void BulkEditSelection<T>(Action<T[]> action, SelectionMode selectionMode = SelectionMode.Unfiltered) where T : Object {
            AssetDatabase.StartAssetEditing();

            action.InvokeSafe(Selection.GetFiltered<T>(selectionMode));

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static void BulkEdit(List<string> guids, Action guidsAction) {
            AssetDatabase.StartAssetEditing();

            guidsAction.InvokeSafe();

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            if (guids[0].LoadAssetByGUID().IsNot<DefaultAsset>())
                AssetDatabase.ImportAsset(guids[0].GUIDToPath(), ImportAssetOptions.ForceUpdate);
        }

        public static void BulkEdit(List<string> guids, Action<string> guidsAction) {
            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();

            AssetDatabase.StartAssetEditing();

            foreach (string guid in guids)
                guidsAction.InvokeSafe(guid);

            AssetDatabase.StopAssetEditing();
            Undo.CollapseUndoOperations(group);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            if (guids[0].LoadAssetByGUID().IsNot<DefaultAsset>())
                AssetDatabase.ImportAsset(guids[0].GUIDToPath(), ImportAssetOptions.ForceUpdate);
        }

    }

}