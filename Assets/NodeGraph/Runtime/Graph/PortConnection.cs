using System;

namespace NodeGraph
{
    [Serializable]
    public class PortConnection
    {
        public string inputPortName;
        public string outputPortName;
        public int inputNodeId;
        public int outputNodeId;
    }
}