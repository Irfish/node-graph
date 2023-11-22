using NodeGraph;
using UnityEngine;
using UnityEngine.Serialization;

namespace Example01
{
    public class TestFlow : MonoBehaviour
    {
        private BaseFlow m_flow;
        [FormerlySerializedAs("mGraphSerializerScriptable")] [FormerlySerializedAs("mNodeGraphScriptable")] [FormerlySerializedAs("m_graph")] public GraphScriptable mGraphScriptable;
        
        private void LoadGraph()
        {
            var obj = Resources.Load<ScriptableObject>("Graph/logic_graph_01");
            if (obj != null)
            {
                if (obj is GraphScriptable graph)
                {
                    mGraphScriptable = graph;
                    var enterNode = graph.graph.enterNode;
                    if (enterNode != null)
                    {
                        var flow = new BaseFlow();
                        flow.Init(enterNode);
                        m_flow = flow;
                        m_flow.Start();
                        InvokeRepeating(nameof(FixedTick), 0, 0.1f);     
                    }
                    else
                    {
                        Debug.LogError("没有EnterNode节点");
                    }
                }
            }
            else
            {
                Debug.LogError("加载失败");
            }
        }

        public void StartFlow()
        {
            if (m_flow != null)
            {
                m_flow.Restart();
            }
            else
            {
                LoadGraph();   
            }
        }

        private void FixedTick()
        {
            if (m_flow != null)
            {
                m_flow.Tick(0.1f);
            }
        }
        
    }
}