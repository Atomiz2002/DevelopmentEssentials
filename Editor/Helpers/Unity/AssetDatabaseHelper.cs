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

        // [Pure]
        // public static List<string> GetSelectedGUIDsRecursively<T>() => GetSelectedGUIDsRecursively("t:" + typeof(T).Name);
        //
        // [Pure]
        // public static List<string> GetSelectedGUIDsRecursively(string filter = "") {
        //     List<string> selectedGUIDs = new(Selection.assetGUIDs);
        //
        //     if (selectedGUIDs.Count == 0)
        //         return new();
        //
        //     foreach (string guid in selectedGUIDs.ToArray()) {
        //         string path = AssetDatabase.GUIDToAssetPath(guid);
        //
        //         if (AssetDatabase.IsValidFolder(path)) // If it's a folder, get all texture GUIDs inside it
        //             selectedGUIDs.AddRange(AssetDatabase.FindAssets(filter, new[] { path }));
        //     }
        //
        //     return selectedGUIDs;
        // }

        public static void BulkEditFilteredSelectionIndividually<T>(Action<T> action, SelectionMode selectionMode = SelectionMode.Unfiltered, string undoName = null) where T : Object =>
            StartStopAssetEditing(undoName, () => Selection.GetFiltered<T>(selectionMode).ForEach<T>(action.InvokeSafe));

        public static void BulkEditFilteredSelection<T>(Action<T[]> action, SelectionMode selectionMode = SelectionMode.Unfiltered, string undoName = null) where T : Object =>
            StartStopAssetEditing(undoName, () => action.InvokeSafe(Selection.GetFiltered<T>(selectionMode)));

        public static void BulkEditGUIDs(List<string> guids, Action guidsAction, string undoName = null) {
            StartStopAssetEditing(undoName, guidsAction);

            if (guids[0].LoadAssetByGUID().IsNot<DefaultAsset>())
                AssetDatabase.ImportAsset(guids[0].GUIDToPath(), ImportAssetOptions.ForceUpdate);
        }

        public static void BulkEditGUIDs(List<string> guids, Action<string> guidsAction, string undoName = null) {
            StartStopAssetEditing(undoName, () => {
                foreach (string guid in guids)
                    guidsAction.InvokeSafe(guid);
            });

            if (guids[0].LoadAssetByGUID().IsNot<DefaultAsset>())
                AssetDatabase.ImportAsset(guids[0].GUIDToPath(), ImportAssetOptions.ForceUpdate);
        }

        private static void StartStopAssetEditing(string undoName, Action action) {
            UndoHelper.Record(undoName, () => {
                AssetDatabase.StartAssetEditing();
                action.InvokeSafe();
                AssetDatabase.StopAssetEditing();
            });

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

    }

}