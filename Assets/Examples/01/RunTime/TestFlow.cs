using NodeGraph;
using UnityEngine;

namespace Example01
{
    public class TestFlow : MonoBehaviour
    {
        private BaseFlow m_flow;
        public BaseGraph m_graph;
        
        private void LoadGraph()
        {
            var obj = Resources.Load<ScriptableObject>("Graph/logic_graph_01");
            if (obj != null)
            {
                if (obj is BaseGraph graph)
                {
                    m_graph = graph;
                    if (graph.enterNode != null)
                    {
                        var flow = new BaseFlow();
                        flow.Init(graph.enterNode);
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