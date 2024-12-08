using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIoutline))]
public class UIOutlineEditor : Editor
{
    private SerializedProperty lineColor;
    private SerializedProperty lineWidth;
    private SerializedProperty drawTop;
    private SerializedProperty drawBottom;
    private SerializedProperty drawLeft;
    private SerializedProperty drawRight;
    private SerializedProperty topStartAlpha;
    private SerializedProperty topEndAlpha;
    private SerializedProperty bottomStartAlpha;
    private SerializedProperty bottomEndAlpha;
    private SerializedProperty leftStartAlpha;
    private SerializedProperty leftEndAlpha;
    private SerializedProperty rightStartAlpha;
    private SerializedProperty rightEndAlpha;

    private bool foldoutEdges = true;
    private bool foldoutAlphas = true;

    private SerializedProperty blinkSpeed;

    private void OnEnable()
    {
        lineColor = serializedObject.FindProperty("lineColor");
        lineWidth = serializedObject.FindProperty("lineWidth");
        drawTop = serializedObject.FindProperty("drawTop");
        drawBottom = serializedObject.FindProperty("drawBottom");
        drawLeft = serializedObject.FindProperty("drawLeft");
        drawRight = serializedObject.FindProperty("drawRight");
        topStartAlpha = serializedObject.FindProperty("topStartAlpha");
        topEndAlpha = serializedObject.FindProperty("topEndAlpha");
        bottomStartAlpha = serializedObject.FindProperty("bottomStartAlpha");
        bottomEndAlpha = serializedObject.FindProperty("bottomEndAlpha");
        leftStartAlpha = serializedObject.FindProperty("leftStartAlpha");
        leftEndAlpha = serializedObject.FindProperty("leftEndAlpha");
        rightStartAlpha = serializedObject.FindProperty("rightStartAlpha");
        rightEndAlpha = serializedObject.FindProperty("rightEndAlpha");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(lineColor, new GUIContent("Line Color"));
        EditorGUILayout.PropertyField(lineWidth, new GUIContent("Line Width"));

        foldoutEdges = EditorGUILayout.Foldout(foldoutEdges, "边的生成控制");
        if (foldoutEdges)
        {
            EditorGUILayout.PropertyField(drawTop);
            EditorGUILayout.PropertyField(drawBottom);
            EditorGUILayout.PropertyField(drawLeft);
            EditorGUILayout.PropertyField(drawRight);
        }

        foldoutAlphas = EditorGUILayout.Foldout(foldoutAlphas, "透明度渐变控制");
        if (foldoutAlphas)
        {
            EditorGUILayout.PropertyField(topStartAlpha);
            EditorGUILayout.PropertyField(topEndAlpha);
            EditorGUILayout.PropertyField(bottomStartAlpha);
            EditorGUILayout.PropertyField(bottomEndAlpha);
            EditorGUILayout.PropertyField(leftStartAlpha);
            EditorGUILayout.PropertyField(leftEndAlpha);
            EditorGUILayout.PropertyField(rightStartAlpha);
            EditorGUILayout.PropertyField(rightEndAlpha);
        }


        serializedObject.ApplyModifiedProperties();
    }
}
