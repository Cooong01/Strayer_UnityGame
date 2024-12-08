using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAI))]
public class EnemyEditor : Editor
{
    public SerializedProperty Blood;
    private SerializedProperty drawLine;
    private SerializedProperty angle;
    private SerializedProperty Deteradius;
    //private SerializedProperty rayCount;
    private SerializedProperty targetLayer;
    private SerializedProperty crossRadius;
    private SerializedProperty waitTime;

    private SerializedProperty FollowDis;
    private SerializedProperty moveSpeed;

    private SerializedProperty obstacleLayer;
    private SerializedProperty groundedLayer;
    private SerializedProperty jumpHeight;

    private bool foldoutDetection = false;
    private bool foldoutChase = false;
    private bool foldoutGravityAndJump = false;

    private void OnEnable()
    {
        drawLine = serializedObject.FindProperty("DrawLine");
        angle = serializedObject.FindProperty("angle");
        Deteradius = serializedObject.FindProperty("Deteradius");
        //rayCount = serializedObject.FindProperty("rayCount");
        targetLayer = serializedObject.FindProperty("targetLayer");
        crossRadius = serializedObject.FindProperty("CrossRadius");
        waitTime = serializedObject.FindProperty("WaitTime");

        FollowDis = serializedObject.FindProperty("FollowDis");
        moveSpeed = serializedObject.FindProperty("MoveSpeed");

        obstacleLayer = serializedObject.FindProperty("obstacleLayer");
        groundedLayer = serializedObject.FindProperty("GroundedLayer");
        jumpHeight = serializedObject.FindProperty("jumpHeight");

        Blood = serializedObject.FindProperty("Blood");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(Blood, new GUIContent("Blood"));
        
        // Detection Settings
        foldoutDetection = EditorGUILayout.Foldout(foldoutDetection, "¼ì²â·¶Î§");
        if (foldoutDetection)
        {
            EditorGUILayout.PropertyField(drawLine, new GUIContent("Draw Line"));
            EditorGUILayout.PropertyField(angle, new GUIContent("Angle"));
            EditorGUILayout.PropertyField(Deteradius, new GUIContent("Deteradius"));
            //EditorGUILayout.PropertyField(rayCount, new GUIContent("Ray Count"));
            EditorGUILayout.PropertyField(targetLayer, new GUIContent("Target Layer"));
            EditorGUILayout.PropertyField(crossRadius, new GUIContent("Cross Radius"));
        }

        // Chase Settings
        foldoutChase = EditorGUILayout.Foldout(foldoutChase, "×·»÷");
        if (foldoutChase)
        {
            EditorGUILayout.PropertyField(FollowDis, new GUIContent("FollowDis"));
            EditorGUILayout.PropertyField(moveSpeed, new GUIContent("Move Speed"));
            EditorGUILayout.PropertyField(waitTime, new GUIContent("Wait Time"));
        }

        // Gravity and Jump Settings
        foldoutGravityAndJump = EditorGUILayout.Foldout(foldoutGravityAndJump, "ÖØÁ¦ÓëÌøÔ¾");
        if (foldoutGravityAndJump)
        {
            EditorGUILayout.PropertyField(obstacleLayer, new GUIContent("Obstacle Layer"));
            EditorGUILayout.PropertyField(groundedLayer, new GUIContent("Grounded Layer"));
            EditorGUILayout.PropertyField(jumpHeight, new GUIContent("Jump Height"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
