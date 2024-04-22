﻿using UnityEditor;
using UnityEngine;

namespace AllieJoe.JuiceIt
{
    [CustomPropertyDrawer(typeof(AudioTuning))]
    public class AudioTuningDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            SerializedProperty keyProperty = property.FindPropertyRelative("Key");
            SerializedProperty clipsProperty = property.FindPropertyRelative("Clips");
            SerializedProperty volumeProperty = property.FindPropertyRelative("Volume");
            
            EditorGUI.BeginProperty(position, label, property);

            Rect keyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect volumeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            Rect clipsRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUI.GetPropertyHeight(clipsProperty));
            
            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(AudioLibrary.TUNING_KEYS, keyProperty.stringValue));
            selectedIndex = EditorGUI.Popup(keyRect, selectedIndex, AudioLibrary.TUNING_KEYS);
            keyProperty.stringValue = AudioLibrary.TUNING_KEYS[selectedIndex];

            volumeProperty.floatValue = EditorGUI.Slider(volumeRect, volumeProperty.name, volumeProperty.floatValue, 0f, 1f);
            EditorGUI.PropertyField(clipsRect, clipsProperty, true);

            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Clips")) + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}