using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData))]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    private ShapeData shapeDataInstance => target as ShapeData;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearListButton();
        EditorGUILayout.Space();

        DrawShapeInputFields();
        EditorGUILayout.Space();

        if(shapeDataInstance.rowsList != null && shapeDataInstance.rows > 0 && shapeDataInstance.columns > 0){
            DrawRowsTable();
        }

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed){
            EditorUtility.SetDirty(shapeDataInstance);
        }
    }

    private void ClearListButton()
    {
        if (GUILayout.Button("Clear List"))
        {
            shapeDataInstance.Clear();
        }
    }

    private void DrawShapeInputFields()
    {
        var columns = shapeDataInstance.columns;
        var rows = shapeDataInstance.rows;

        shapeDataInstance.columns = EditorGUILayout.IntField("Columns", shapeDataInstance.columns);
        shapeDataInstance.rows = EditorGUILayout.IntField("Rows", shapeDataInstance.rows);

        if ((columns != shapeDataInstance.columns || rows != shapeDataInstance.rows) && shapeDataInstance.columns > 0 && shapeDataInstance.rows > 0)
        {
            shapeDataInstance.CreateTable();
        }
    }

    private void DrawRowsTable(){
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        var headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;

        var headerRowStyle = new GUIStyle();
        headerRowStyle.fixedHeight = 25;
        headerRowStyle.alignment = TextAnchor.MiddleCenter;

        var dataStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataStyle.normal.background = Texture2D.grayTexture;
        dataStyle.onNormal.background = Texture2D.whiteTexture;

        for(var row = 0; row < shapeDataInstance.rows; row++){
            EditorGUILayout.BeginHorizontal(headerColumnStyle);
            for(var column = 0; column < shapeDataInstance.columns; column++){
                EditorGUILayout.BeginHorizontal(headerRowStyle);
                var data = EditorGUILayout.Toggle(shapeDataInstance.rowsList[row].column[column], dataStyle);
                shapeDataInstance.rowsList[row].column[column] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}