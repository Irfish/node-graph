using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public abstract class BaseGraphWindow : EditorWindow
    {
        private const string graphWindowStylePath = "Assets/NodeGraph/Editor/Src/Styles/BaseGraphView.uss";
        protected VisualElement rootView;
        protected BaseGraphView graphView;
        protected BaseGraph graph;

        protected virtual void OnEnable()
        {
            InitRootView();
        }
        
        protected virtual void Update()
        {
            var obj = Selection.activeObject;
            if (obj != null && obj is BaseGraph g)
            {
                if(g!=graph)
                {
                    InitGraph(g);
                }
            }
        }
        
        private void InitRootView()
        {
            rootView = base.rootVisualElement;
            rootView.name = "graphRootView";
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(graphWindowStylePath);
            rootView.styleSheets.Add(styleSheet);
        }

        protected void InitGraph(BaseGraph g)
        {
            graph = g;

            if (graphView != null)
                rootView.Remove(graphView);

            InitWindow(graph);
            graphView = rootView.Children().FirstOrDefault(e => e is BaseGraphView) as BaseGraphView;
            if (graphView != null)
            {
                graphView.Init(graph);
            }
            else
            {
                Debug.LogError("graphView 不存在");
            }
        }

        protected abstract void InitWindow(BaseGraph graph);
    }

}