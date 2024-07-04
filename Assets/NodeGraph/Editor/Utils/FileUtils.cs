using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeGraph.Editor
{
    public static class FileUtils
    {
        private const string DEFAULT_PATH = "Assets";

        public static string SelectFilePath(string fileName)
        {
            var path = "";
            var fullPath = EditorUtility.SaveFilePanel("保存", DEFAULT_PATH, fileName, "asset");
            if (!string.IsNullOrEmpty(fullPath))
            {
                var index = fullPath.IndexOf(DEFAULT_PATH, StringComparison.Ordinal);
                if (index != -1)
                {
                    path = fullPath.Substring(index);
                    return path;
                }

                return fullPath;
            }

            return path;
        }
        
        public static void SaveGraph(IGraphSerializer graphSerializer)
        {
            if (graphSerializer is ScriptableObject obj)
            {
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("提示", "保存成功", "OK");
                var path = AssetDatabase.GetAssetPath(obj);
                var m = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                EditorGUIUtility.PingObject(m);    
            }
            else if (graphSerializer is MonoBehaviour mono)
            {
                var go = mono.gameObject;
                if (EditorUtility.IsPersistent(go))
                {
                    PrefabUtility.SavePrefabAsset(go);
                }
                else
                {
                    PrefabUtility.ApplyPrefabInstance(go,InteractionMode.AutomatedAction);
                }
            }
        }
        
        public static void SaveGraphAsNew(IGraphSerializer graphSerializer)
        {
            var path = SelectFilePath(graphSerializer.graphName);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var arr = path.Split('/');
            if (arr.Length > 0)
            {
                var arr1 = arr[^1].Split(".");
                graphSerializer.graphName = arr1[0];
                if (graphSerializer is ScriptableObject obj)
                {
                    var copy = Object.Instantiate(obj);
                    copy.name = obj.name;
                    AssetDatabase.CreateAsset(copy, path);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("提示", "保存成功", "OK");
                    var m = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    EditorGUIUtility.PingObject(m); 
                }
            }
        }
    }
}