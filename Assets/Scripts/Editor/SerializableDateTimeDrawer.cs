using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public class SerializableDateTimeDrawer : OdinValueDrawer<SerializableDateTime>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            SerializableDateTime value = this.ValueEntry.SmartValue;
            var modifiedString = EditorGUI.DelayedTextField(rect, value.ToString());
            try
            {
                this.ValueEntry.SmartValue = SerializableDateTime.FromString(modifiedString);
            }catch
            {
                // ignored
            }
        }
    }
}