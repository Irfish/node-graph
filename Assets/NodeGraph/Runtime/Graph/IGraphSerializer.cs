namespace NodeGraph
{
    public interface IGraphSerializer
    {
        public GraphData graph { get; }

#if UNITY_EDITOR
        public string graphName { get; set; }
#endif
    }
}