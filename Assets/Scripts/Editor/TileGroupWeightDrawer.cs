using UnityEngine;
using UnityEditor;

namespace AllieJoe.JuiceIt
{
    [CustomPropertyDrawer(typeof(GridTileTuningSO.TileGroupWeight))]
    public class TileGroupWeightDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the sprite property
            SerializedProperty spriteProperty = property.FindPropertyRelative("Sprite");
            SerializedProperty weightProperty = property.FindPropertyRelative("Weight");
            
            Sprite sprite = spriteProperty.objectReferenceValue as Sprite;
            float aspectRatio = 1f;
            if (sprite != null)
                aspectRatio = (float)sprite.texture.width / sprite.texture.height;
            
            EditorGUI.BeginProperty(position, label, property);

            float imageWidth = position.height * aspectRatio;
            float weightOffset = imageWidth + 10f;
            
            // Split the position into two parts: one for the sprite preview and one for the weight field
            Rect spriteRect = new Rect(position.x, position.y, imageWidth, position.height);
            Rect weightRect = new Rect(position.x + weightOffset, position.y, position.width - weightOffset, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.ObjectField(spriteRect, spriteProperty, typeof(Sprite), GUIContent.none);
            EditorGUI.PropertyField(weightRect, weightProperty, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Return the height of one line (sprite preview and weight field)
            return EditorGUIUtility.singleLineHeight * 4;
        }
    }
}