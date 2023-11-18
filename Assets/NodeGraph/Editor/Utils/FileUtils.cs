using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace NodeGraph.Editor
{
    public static class FileUtils
    {
        private const string DEFAULT_PATH = "Assets/Resources/Graph";

        private static string SelectFilePath(string fileName)
        {
            var path = "";
            var fullPath = EditorUtility.SaveFilePanel("保存", DEFAULT_PATH, fileName, "asset");
            if (!string.IsNullOrEmpty(fullPath))
            {
                var index = fullPath.IndexOf(DEFAULT_PATH, StringComparison.Ordinal);
                path = fullPath.Substring(index);
            }

            return path;
        }
        
        public static void SaveGraph(BaseGraph graph)
        {
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("提示", "保存成功", "OK");
            var path = AssetDatabase.GetAssetPath(graph);
            var m = AssetDatabase.LoadAssetAtPath<BaseGraph>(path);
            EditorGUIUtility.PingObject(m);
        }
        
        public static void SaveGraphAsNew(BaseGraph graph)
        {
            var path = SelectFilePath(graph.name);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var arr = path.Split('/');
            if (arr.Length > 0)
            {
                var arr1 = arr[^1].Split(".");
                graph.name = arr1[0];
                var copy = Object.Instantiate(graph);
                copy.name = graph.name;
                AssetDatabase.CreateAsset(copy, path);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("提示", "保存成功", "OK");
                var m = AssetDatabase.LoadAssetAtPath<BaseGraph>(path);
                EditorGUIUtility.PingObject(m);
            }
        }
    }
}