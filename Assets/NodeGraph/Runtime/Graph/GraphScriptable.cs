using UnityEngine;

namespace NodeGraph
{
    public class GraphScriptable : ScriptableObject, IGraphSerializer
    {
        [SerializeField] private GraphData m_graph;

        public GraphData graph
        {
            get { return m_graph ??= new GraphData(); }
        }

#if UNITY_EDITOR
        public string graphName
        {
            set => name = value;
            get => name;
        }
#endif
    }
}