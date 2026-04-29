#if DEVELOPMENT_ESSENTIALS_EDITOR_ODIN_INSPECTOR
using System;
using DevelopmentEssentials.Extensions.Attributes;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace DevelopmentEssentials.Editor.Attributes {

    public class TimestampAttributeDrawer<T> : OdinAttributeDrawer<TimestampAttribute, T> {

        protected override void DrawPropertyLayout(GUIContent label) {
            CallNextDrawer(label);

            long timestamp;

            switch (ValueEntry.SmartValue) {
                case string s:
                    if (!long.TryParse(s, out long t))
                        return;

                    timestamp = t;
                    break;
                case int i:
                    timestamp = i;
                    break;
                case long l:
                    timestamp = l;
                    break;
                default:
                    return;
            }

            Rect unitRect = GUILayoutUtility.GetLastRect();
            unitRect.xMax -= 5;
            GUIStyle style = new(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleRight };

            try {
                (string unit, DateTimeOffset offset) = Attribute.format switch {
                    TimestampAttribute.Format.Seconds      => ("s", DateTimeOffset.FromUnixTimeSeconds(timestamp)),
                    TimestampAttribute.Format.Milliseconds => ("m", DateTimeOffset.FromUnixTimeMilliseconds(timestamp)),
                    TimestampAttribute.Format.Ticks        => ("t", DateTimeOffset.FromUnixTimeMilliseconds(timestamp * TimeSpan.TicksPerMillisecond)),
                    _                                      => (string.Empty, DateTimeOffset.MinValue)
                };

                // EditorGUI.LabelField(unitRect, new GUIContent(unit, timestamp < 0 || offset == DateTimeOffset.MinValue ? "Invalid" : $"{offset:h:mm:ss M/d/yyyy}"), style);
                EditorGUI.LabelField(unitRect, new GUIContent(timestamp < 0 || offset == DateTimeOffset.MinValue ? "Invalid" : $"{offset:h:mm:ss d/M/yyyy}", unit), style);
            }
            catch (ArgumentOutOfRangeException) {
                style.normal.textColor = Color.red;
                EditorGUI.LabelField(unitRect, new GUIContent("Invalid value or unit"), style);
            }

        }

    }

}
#endif