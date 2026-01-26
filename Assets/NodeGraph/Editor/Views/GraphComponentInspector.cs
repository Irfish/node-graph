using NodeGraph.Editor;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomEditor(typeof(GraphComponent))] 
    public class GraphComponentInspector :UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var graph = serializedObject.targetObject as GraphComponent;
            if (graph != null)
            {
                if (GUILayout.Button("open graph"))
                {
                    OpenGraph(graph);
                }
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawInspectorGUI();
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OpenGraph(GraphComponent graph)
        {
            DefaultGraphWindow.OpenWithGraph(graph);
        }
        
        protected virtual void DrawInspectorGUI()
        {
            
        }
    }
}

