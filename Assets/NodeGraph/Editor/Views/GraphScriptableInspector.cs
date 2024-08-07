using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomEditor( typeof( GraphScriptable ) )]
    public class GraphScriptableInspector:UnityEditor.Editor
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
                var t = serializedObject.targetObject as GraphScriptable;
                BaseGraphWindow.OpenWithGraph<DefaultGraphWindow>( t );
            }
        }
    } 
}
