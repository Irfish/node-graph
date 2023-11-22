using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class GraphData : ISerializationCallbackReceiver
    {
        private EnterNode m_enterNode;

        [SerializeReference] private List<BaseNode> m_nodes = new();

        [SerializeField] private List<PortConnection> m_connections = new();

        public List<BaseNode> nodes => m_nodes;

        public EnterNode enterNode => m_enterNode;

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            OnDeserialize();
        }

        #endregion

        private void OnDeserialize()
        {
            Dictionary<int, BaseNode> guidToNode = new();

            foreach (var node in m_nodes)
            {
                node.Reset();
                guidToNode[node.id] = node;
                if (node is EnterNode enter)
                {
                    m_enterNode = enter;
                }
            }

            foreach (var connection in m_connections)
            {
                guidToNode.TryGetValue(connection.inputNodeId, out var inputNode);
                guidToNode.TryGetValue(connection.outputNodeId, out var outputNode);
                if (inputNode != null && outputNode != null)
                {
                    var inputPort = inputNode.GetPort(NodePortType.Input, connection.inputPortName);
                    var outputPort = outputNode.GetPort(NodePortType.Output, connection.outputPortName);
                    if (inputPort != null && outputPort != null)
                    {
                        inputPort.LinkTo(outputPort);
                        outputPort.LinkTo(inputPort);
                    }
                }
            }
        }


#if UNITY_EDITOR
        public List<PortConnection> connections => m_connections;

        public void Reset()
        {
            nodes.Clear();
            connections.Clear();
        }

        public void AddNode(BaseNode node)
        {
            nodes.Add(node);
        }

        public void AddConnection(PortConnection connection)
        {
            connections.Add(connection);
        }
#endif
    }
}