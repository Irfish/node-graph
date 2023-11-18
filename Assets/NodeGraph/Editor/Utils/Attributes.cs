using System;

namespace NodeGraph.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeTargetEditor : Attribute
    {
        public readonly Type nodeType;

        public NodeTargetEditor(Type nodeType)
        {
            this.nodeType = nodeType;
        }
    }
}
