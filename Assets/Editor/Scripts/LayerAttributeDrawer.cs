﻿using UnityEditor;
using UnityEngine;

namespace utilities.general.attributes
{
    public class LayerAttributeDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}
