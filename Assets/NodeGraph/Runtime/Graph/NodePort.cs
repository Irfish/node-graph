using System.Collections.Generic;

namespace NodeGraph
{
    public enum NodePortType
    {
        Input,
        Output
    }

    public struct PortData
    {
        public readonly string name;
        public readonly bool multiLink;
        public readonly bool vertical;

        public PortData(string name, bool multiLink = false)
        {
            this.name = name;
            this.multiLink = multiLink;
            this.vertical = false;
        }
    }

    public sealed class NodePort
    {
        private readonly PortData data;
        private readonly BaseNode owner;
        private readonly List<NodePort> connections = new();
        public string portName => data.name;
        
        public NodePort(BaseNode owner, PortData data)
        {
            this.owner = owner;
            this.data = data;
        }

        public void LinkTo(NodePort port)
        {
            if (!connections.Contains(port))
            {
                connections.Add(port);
            }
        }

        public void Delink(NodePort port)
        {
            connections.Remove(port);
        }

        public void Impulse(FlowContext ctx)
        {
            for (int i = 0, cnt = connections.Count; i < cnt; i++)
            {
                var port = connections[i];
                var ownerNode = port.owner;
                if (ownerNode != null)
                {
                    ownerNode.ImpulseInPort(port.portName, ctx);
                    ctx.TryAddNode(ownerNode);
                }
            }
        }
        
        public void CollectConnectionNodes(List<BaseNode> nodes)
        {
            foreach (var port in connections)
            {
                var n = port.owner;
                if (n != null)
                {
                    nodes.Add(n);
                    n.CollectConnectionNodes(nodes);
                }
            }
        }
    }
}