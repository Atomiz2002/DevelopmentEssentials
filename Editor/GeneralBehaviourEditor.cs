#if DEVELOPMENT_ESSENTIALS_COMPONENT_NAMES
using System.Linq;
using System.Reflection;
using ComponentNames.Editor;
using DevelopmentEssentials.Extensions.CS;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Editor {

    [InitializeOnLoad]
    public static class ReferencedScriptValidator {

        static ReferencedScriptValidator() {
            ComponentHeader.AfterHeaderGUI -= AfterInspectorRootEditorHeaderGUI;
            ComponentHeader.AfterHeaderGUI += AfterInspectorRootEditorHeaderGUI;
        }

        private static float AfterInspectorRootEditorHeaderGUI(Component component, Rect headerRect, bool headerIsSelected, bool supportsRichText) {
            const int downsize = 4;
            headerRect.x      += headerRect.width - headerRect.height * 4;
            headerRect.x      += downsize * 2;
            headerRect.y      += downsize;
            headerRect.height -= downsize * 2;
            headerRect.width  =  headerRect.height;

            if (component.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.IsDefined(typeof(RequiredAttribute), true))
                .Any(f => f.GetValue(component).Out(out object o) is Object u ? !u : o == null)) {
                SdfIcons.DrawIcon(headerRect, SdfIconType.XOctagonFill, Color.softRed);
            }

            return 0; // unused
        }

    }

}
#endif