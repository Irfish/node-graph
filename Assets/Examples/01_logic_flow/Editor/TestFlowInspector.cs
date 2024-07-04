using NodeGraph.Editor;
using UnityEditor;
using UnityEngine;

namespace Example01.Editor
{
    [CustomEditor(typeof(TestFlow))]
    public class TestFlowInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var testFlow = serializedObject.targetObject as TestFlow;
            if (testFlow != null)
            {
                if (GUILayout.Button("Start"))
                {
                    testFlow.StartFlow();
                }

                if (GUILayout.Button("open graph window"))
                {
                    var graph = testFlow.mGraphScriptable;
                    if (graph != null)
                    {
                        BaseGraphWindow.OpenWithGraph<LogicFlowGraphWindow>(graph);
                    }
                    else
                    {
                        Debug.LogError("flow 未初始化");
                    }
                }
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
        
    }
}