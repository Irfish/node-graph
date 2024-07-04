using NodeGraph;
using NodeGraph.Editor;
using UnityEditor.Experimental.GraphView;

namespace Example01.Editor
{
    [NodeTargetEditor(typeof(PrintNode))]
    public class PrintNodeEditor : BaseNodeView
    {
        protected override void OnInit()
        {
            
        }
    }
}

