using System;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    private Node m_Node;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        m_Node = (Node)target;
        
        EditorGUILayout.LabelField("Puzzle");
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < m_Node.m_Puzzle.GetLength(0); i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < m_Node.m_Puzzle.GetLength(1); j++)
                EditorGUILayout.TextField(GetNumber(m_Node.m_Puzzle[i, j]));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    private string GetNumber(int value)
    {
        if (value > 0)
            return value.ToString();

        return "";
    }
}