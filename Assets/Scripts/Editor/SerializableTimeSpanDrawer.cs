using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public class SerializableTimeSpanDrawer : OdinValueDrawer<SerializableTimeSpan>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            SerializableTimeSpan value = this.ValueEntry.SmartValue;
            var modifiedString = EditorGUI.DelayedTextField(rect, value.ToString());
            try
            {
                this.ValueEntry.SmartValue = SerializableTimeSpan.FromString(modifiedString);
            }catch
            {
                // ignored
            }
        }
    }
}