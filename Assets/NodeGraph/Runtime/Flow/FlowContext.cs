using System.Collections.Generic;

namespace NodeGraph
{
    public class FlowContext
    {
        private readonly BaseFlow m_flow;
        private readonly List<BaseNode> m_activeNodes = new();
        private readonly HashSet<int> m_nodeCheck = new();
        
        public BaseFlow flow => m_flow;

        public List<BaseNode> activeNodes => m_activeNodes;

        public FlowContext(BaseFlow baseflow)
        {
            m_flow = baseflow;
        }
        
        public void TryAddNode(BaseNode node)
        {
            if (m_nodeCheck.Add(node.id))
            {
                m_activeNodes.Add(node);
            }
        }

        public void Reset()
        {
            m_nodeCheck.Clear();
            m_activeNodes.Clear();
        }
    }
}