using System;
using DevelopmentEssentials.Extensions.CS;
using UnityEditor;

namespace DevelopmentEssentials.Editor.Helpers.Unity {

    public static class UndoHelper {

        public static void BeginRecord(out int group, string name = "") {
            Undo.IncrementCurrentGroup();
            if (!string.IsNullOrWhiteSpace(name))
                Undo.SetCurrentGroupName(name);
            group = Undo.GetCurrentGroup();
        }

        public static void EndRecord(int group) => Undo.CollapseUndoOperations(group);

        public static void Record(Action action) => Record(null, action);

        public static void Record(string name, Action action) {
            BeginRecord(out int group, name);
            action.InvokeSafe();
            EndRecord(group);
        }

    }

}