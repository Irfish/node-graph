using UnityEditor;
using UnityEngine;

namespace NodeGraph.Editor
{
    public class DefaultGraphWindow : BaseGraphWindow
    {
        protected override void InitWindow(BaseGraph g)
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
            var graph = ScriptableObject.CreateInstance<BaseGraph>();
            graph.hideFlags = HideFlags.HideAndDontSave;
            graph.name = "logic_graph_new";
            OpenWithGraph(graph);
        }

        public static void OpenWithGraph(BaseGraph graph)
        {
            var graphWindow = CreateWindow<DefaultGraphWindow>();
            graphWindow.InitGraph(graph);
            graphWindow.Show();
        }
    }
}