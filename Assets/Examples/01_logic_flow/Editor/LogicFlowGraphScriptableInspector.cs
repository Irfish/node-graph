using NodeGraph;
using NodeGraph.Editor;
using UnityEditor;
using UnityEngine;

namespace Example01.Editor
{
    [CustomEditor( typeof( LogicFlowGraphScriptable ) )]
    public class LogicFlowGraphScriptableInspector : GraphScriptableInspector
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawOpenGraphWindow();
            
            GUILayout.Space( EditorGUIUtility.singleLineHeight );
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawOpenGraphWindow()
        {
            if ( GUILayout.Button( "open graph" ) ) {
                var t = serializedObject.targetObject as LogicFlowGraphScriptable;
                BaseGraphWindow.OpenWithGraph<LogicFlowGraphWindow>( t );
            }
        }
        
        [MenuItem("Assets/Create/Scriptable/LogicFlowGraph")]
        private static void CreateLogicFlowGraphScriptable()
        {
            var path = FileUtils.SelectFilePath("new_logic_flow_graph");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var arr = path.Split('/');
            if (arr.Length > 0)
            {
                var arr1 = arr[^1].Split(".");
                var copy = ScriptableObject.CreateInstance<LogicFlowGraphScriptable>();
                copy.name = arr1[0];
                AssetDatabase.CreateAsset(copy, path);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("提示", "保存成功", "OK");
                var m = AssetDatabase.LoadAssetAtPath<LogicFlowGraphScriptable>(path);
                EditorGUIUtility.PingObject(m);
            }
        }
    }
    
}

