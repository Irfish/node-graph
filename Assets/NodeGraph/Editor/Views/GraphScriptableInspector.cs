using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomEditor( typeof( GraphScriptable ) )]
    public class GraphScriptableInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            if ( GUILayout.Button( "open graph" ) )
            {
                OpenGraph(serializedObject.targetObject as GraphScriptable);
            }
            
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawInspectorGUI();
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInspectorGUI()
        {
            
        }
        
        protected virtual void OpenGraph(GraphScriptable graph)
        {
            DefaultGraphWindow.OpenWithGraph(graph);
        }
    } 
}
