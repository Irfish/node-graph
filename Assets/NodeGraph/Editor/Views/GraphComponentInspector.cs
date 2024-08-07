using NodeGraph.Editor;
using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomEditor(typeof(GraphComponent))] 
    public class GraphComponentInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawOpenGraphWindow();
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawOpenGraphWindow()
        {
            var graph = serializedObject.targetObject as GraphComponent;
            if (graph != null)
            {
                if (GUILayout.Button("open graph"))
                {
                    BaseGraphWindow.OpenWithGraph<DefaultGraphWindow>(graph);
                }
            }
        }
    }
}

