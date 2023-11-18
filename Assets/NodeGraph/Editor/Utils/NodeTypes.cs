using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace NodeGraph.Editor
{
    public class NodeMenu
    {
        public string menuPath;
        public string nodeTitle;
        public Type viewType;
        public Type nodeType;
    }

    public static class NodeTypes
    {
        //nodeType -> viewType 
        private static readonly Dictionary<Type, Type> nodeTypeToViewType = new();

        //viewType -> nodeType
        private static readonly Dictionary<Type, Type> viewTypeToNodeType = new();

        //viewType -> menu
        private static readonly Dictionary<Type, NodeMenu> viewTypeToMenu = new();

        //所有可用的节点
        private static readonly List<NodeMenu> nodeMenus = new();

        private static bool TryGetAttribute<T>(Type type, out T att) where T : Attribute
        {
            if (type.GetCustomAttributes(typeof(T), false) is T[] atts)
            {
                if (atts.Length > 0)
                {
                    att = atts.First();
                    return true;
                }
            }

            att = default;
            return false;
        }

        private static void CacheTypes()
        {
            viewTypeToNodeType.Clear();
            nodeTypeToViewType.Clear();
            nodeMenus.Clear();
            foreach (var viewType in TypeCache.GetTypesDerivedFrom<BaseNodeView>())
            {
                if (!viewType.IsAbstract)
                {
                    if (TryGetAttribute<NodeTargetEditor>(viewType, out var targetAtt))
                    {
                        Type nodeType = targetAtt.nodeType;
                        if (TryGetAttribute<NodeMenuAttribute>(nodeType, out var menuAtt))
                        {
                            var menu = new NodeMenu()
                            {
                                menuPath = menuAtt.menuPath,
                                nodeTitle = menuAtt.nodeTitle,
                                nodeType = nodeType,
                                viewType = viewType
                            };
                            nodeMenus.Add(menu);
                            viewTypeToMenu[viewType] = menu;
                            nodeTypeToViewType[nodeType] = viewType;
                            viewTypeToNodeType[viewType] = nodeType;
                        }
                    }
                }
            }
        }

        private static void CheckAndCache()
        {
            if (nodeMenus.Count == 0)
            {
                CacheTypes();
            }
        }

        public static Type GetNodeType(Type viewType)
        {
            CheckAndCache();
            viewTypeToNodeType.TryGetValue(viewType, out var nodeType);
            return nodeType;
        }

        public static Type GetViewType(Type nodeType)
        {
            CheckAndCache();
            nodeTypeToViewType.TryGetValue(nodeType, out var viewType);
            return viewType;
        }

        public static NodeMenu GetMenu(Type viewType)
        {
            CheckAndCache();
            viewTypeToMenu.TryGetValue(viewType, out var menu);
            return menu;
        }

        public static List<NodeMenu> GetNodeMenus()
        {
            CheckAndCache();
            return nodeMenus;
        }
    }
}