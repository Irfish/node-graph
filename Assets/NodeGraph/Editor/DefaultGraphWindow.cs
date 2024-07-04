using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class DefaultGraphWindow : BaseGraphWindow
    {
        protected override void InitWindow(IGraphSerializer g)
        {
            titleContent = new GUIContent("Default Graph");
            if (graphView == null)
            {
                graphView = new BaseGraphView(this);
                graphView.Add(new GraphToolbarView(graphView));
            }

            rootView.Add(graphView);
        }

        [MenuItem("Graph/01 DefaultGraph")]
        public static void OpenWithNewGraph()
        {
            var graph = ScriptableObject.CreateInstance<GraphScriptable>();
            graph.hideFlags = HideFlags.HideAndDontSave;
            graph.name = "logic_graph_new";
            OpenWithGraph<DefaultGraphWindow>(graph);
        }

    }
}