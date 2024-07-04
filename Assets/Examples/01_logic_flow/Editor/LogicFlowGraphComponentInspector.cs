using NodeGraph.Editor;
using UnityEditor;
using UnityEngine;

namespace Example01.Editor
{
    [CustomEditor(typeof(LogicFlowGraphComponent))] 
    public class LogicFlowGraphComponentInspector : GraphComponentInspector
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
            var graph = serializedObject.targetObject as LogicFlowGraphComponent;
            if (graph != null)
            {
                if (GUILayout.Button("open graph"))
                {
                    BaseGraphWindow.OpenWithGraph<LogicFlowGraphWindow>(graph);
                }
            }
        }  
    }
    
}
