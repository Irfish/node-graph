using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    [CustomEditor( typeof( BaseGraph ) )]
    public class BaseGraphInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            if ( GUILayout.Button( "open graph" ) ) {
                var t = serializedObject.targetObject as BaseGraph;
                DefaultGraphWindow.OpenWithGraph( t );
            }
            
            GUILayout.Space( EditorGUIUtility.singleLineHeight );
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    } 
}
