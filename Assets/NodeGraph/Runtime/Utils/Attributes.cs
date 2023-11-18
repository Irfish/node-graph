using System;

namespace NodeGraph
{
    public class NodeMenuAttribute : Attribute
    {
        public string menuPath;
        public string nodeTitle;

        public NodeMenuAttribute(string path, string title = null)
        {
            menuPath = path;
            nodeTitle = title;
        }
    }
}

