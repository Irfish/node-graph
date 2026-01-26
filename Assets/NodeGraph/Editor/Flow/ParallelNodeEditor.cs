using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    [NodeTargetEditor(typeof(ParallelNode))]
    public class ParallelNodeEditor : BaseNodeView
    {
        private ParallelNode node => target as ParallelNode; 
        protected override void OnInit()
        {
            DrawButton("添加输出端口", () =>
            {
                node.AddOutPort();
                graphView.RefreshGraph();
            });
            
            DrawButton("删除输出端口", () =>
            {
                node.RemoveOutPort();
                graphView.RefreshGraph();
            });
        }


    }
}
