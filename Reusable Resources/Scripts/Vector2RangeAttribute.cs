#nullable enable

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Utility.Development
{
    [CustomPropertyDrawer(typeof(Vector2RangeAttribute))]
    public class Vector2RangeAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Vector2RangeAttribute rangeAttribute = (Vector2RangeAttribute)attribute;

            Rect textFieldPosition = position;
            textFieldPosition.width = position.width;
            textFieldPosition.height = position.height;

            EditorGUI.BeginChangeCheck();
            Vector2 val = EditorGUI.Vector2Field(textFieldPosition, label, property.vector2Value);
            if (EditorGUI.EndChangeCheck())
            {
                val.x = Mathf.Clamp(val.x, rangeAttribute.minX, rangeAttribute.maxX);
                val.y = Mathf.Clamp(val.y, rangeAttribute.minY, rangeAttribute.maxY);
                property.vector2Value = val;
            }
        }
    }
}
#endif

namespace Utility.Development
{
    public class Vector2RangeAttribute : PropertyAttribute
    {
        public readonly float minX, maxX, minY, maxY;

        public Vector2RangeAttribute(float minX, float maxX, float minY, float maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }
    }
}
