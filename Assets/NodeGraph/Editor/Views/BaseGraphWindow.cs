using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public abstract class BaseGraphWindow : EditorWindow
    {
        protected VisualElement rootView;
        protected BaseGraphView graphView;
        protected IGraphSerializer GraphSerializer;

        protected virtual void OnEnable()
        {
            InitRootView();
        }
        
        protected virtual void Update()
        {
            var obj = Selection.activeObject;
            if (obj != null && obj is IGraphSerializer g)
            {
                if(g!=GraphSerializer)
                {
                    InitGraph(g);
                }
            }
        }
        
        private void InitRootView()
        {
            rootView = base.rootVisualElement;
            rootView.name = "graphRootView";
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SrcDefine.graphWindowStylePath);
            rootView.styleSheets.Add(styleSheet);
        }

        protected void InitGraph(IGraphSerializer g)
        {
            GraphSerializer = g;

            if (graphView != null)
                rootView.Remove(graphView);

            InitWindow(GraphSerializer);
            graphView = rootView.Children().FirstOrDefault(e => e is BaseGraphView) as BaseGraphView;
            if (graphView != null)
            {
                graphView.Init(GraphSerializer);
            }
            else
            {
                Debug.LogError("graphView 不存在");
            }
        }

        protected abstract void InitWindow(IGraphSerializer graphSerializer);
        
        public static void OpenWithGraph<T>(IGraphSerializer graphSerializer) where T : BaseGraphWindow
        {
            var graphWindow = CreateWindow<T>();
            graphWindow.InitGraph(graphSerializer);
            graphWindow.Show();
        }
    }

}