using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class BaseGraph : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeReference] private List<BaseNode> m_nodes = new();

        [SerializeField] private List<PortConnection> m_connections = new();
        public List<BaseNode> nodes => m_nodes;
        
        [NonSerialized]
        public EnterNode enterNode;
            
        #region ISerializationCallbackReceiver
        
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Dictionary<int, BaseNode> guidToNode = new();
            
            foreach (var node in m_nodes)
            {
                node.InitWithGraph(this);
                guidToNode[node.id] = node;
                if (node is EnterNode enter)
                {
                    enterNode = enter;
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

        #endregion

        public List<PortConnection> connections => m_connections;

#if UNITY_EDITOR
       
        public void AddNode(BaseNode node)
        {
            m_nodes.Add(node);
        }

        public void AddConnection(PortConnection connection)
        {
            m_connections.Add(connection);
        }
#endif
    }
}