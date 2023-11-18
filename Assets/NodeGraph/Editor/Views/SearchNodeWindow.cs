using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace NodeGraph.Editor
{
    public class SearchNodeWindow : ScriptableObject, ISearchWindowProvider
    {
        public delegate bool OnSelectMenuEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context);

        public OnSelectMenuEntry onSelectMenuEntryHandler;
        //保证menu对齐
        private Texture2D m_menuIcon;
        
        private Texture2D menuIcon
        {
            get
            {
                if (m_menuIcon == null)
                {
                    m_menuIcon = new Texture2D(1, 1);
                    m_menuIcon.SetPixel(0, 0, Color.clear);
                    m_menuIcon.Apply();
                }

                return m_menuIcon;
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            var list = NodeTypes.GetNodeMenus();
            var paths = new HashSet<string>();
            foreach (var menu in list)
            {
                var path = menu.menuPath;
                var arr = path.Split('/');
                var count = arr.Length;
                var lv = 0;
                var menuName = path;
                if (count > 1)
                {
                    lv++;
                    menuName = arr[count - 1];
                    var menuPath = "";
                    for (var i = 0; i < count - 1; i++)
                    {
                        lv = i + 1;
                        var p = arr[i];
                        menuPath += p;
                        if (!paths.Contains(menuPath))
                        {
                            paths.Add(menuPath);
                            entries.Add(new SearchTreeGroupEntry(new GUIContent(p),lv));
                        }
                    }
                }

                entries.Add(new SearchTreeEntry(new GUIContent(menuName, menuIcon))
                    { level = lv + 1, userData = menu });
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (onSelectMenuEntryHandler == null)
            {
                return false;
            }

            return onSelectMenuEntryHandler.Invoke(SearchTreeEntry, context);
        }
    }
}