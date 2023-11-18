using UnityEngine;

namespace NodeGraph.Editor
{
    public class GraphToolbarView : ToolbarView
    {
        public GraphToolbarView(BaseGraphView graphView) : base(graphView)
        {
        }

        protected override void AddButtons()
        {
            AddButton("Save As", () =>
            {
                graphView.SaveGraphToDiskAsNew();
            });
            
            AddButton("Save", () =>
            {
                graphView.SaveGraphToDisk();
            });
            
            AddButton("Refresh", () =>
            {
                graphView.RefreshGraph();
            },false);
            
            AddToggle( new GUIContent("编辑模式"), graphView.editorModel, (isOn) =>
            {
                graphView.editorModel = isOn;
            },false);

            base.AddButtons();
        }
    }
}