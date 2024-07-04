using System.Collections;
using System.Collections.Generic;
using NodeGraph;
using UnityEngine;
using NodeGraph.Editor;

namespace Example01.Editor
{
    public class LogicFlowGraphWindow : BaseGraphWindow
    {
        protected override void InitWindow(IGraphSerializer g)
        {
            titleContent = new GUIContent("Logic Flow");
            if (graphView == null)
            {
                graphView = new BaseGraphView(this);
                graphView.Add(new GraphToolbarView(graphView));
            }

            rootView.Add(graphView);
        }
        
        
    }
    
}
