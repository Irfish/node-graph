using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class ToolbarView : VisualElement
    {
        protected enum ElementType
        {
            Button,
            Toggle,
            DropDownButton,
            Separator,
            Custom,
            FlexibleSpace,
        }

        protected class ToolbarButtonData
        {
            public GUIContent content;
            public ElementType type;
            public bool value;
            public bool visible = true;
            public Action buttonCallback;
            public Action<bool> toggleCallback;
            public int size;
            public Action customDrawFunction;
        }

        protected readonly BaseGraphView graphView;
        protected readonly List<ToolbarButtonData> leftButtonDatas = new();
        protected readonly List<ToolbarButtonData> rightButtonDatas = new();

        protected ToolbarView(BaseGraphView graphView)
        {
            name = "ToolbarView";
            this.graphView = graphView;

            graphView.initEvents += () =>
            {
                leftButtonDatas.Clear();
                rightButtonDatas.Clear();
                AddButtons();
            };

            Add(new IMGUIContainer(DrawImGUIToolbar));
        }
        
        protected virtual void AddButtons()
        {
            AddButton("Center", graphView.ResetPositionAndZoom);

            AddButton("Show In Project", () => EditorGUIUtility.PingObject(graphView.graph), false);
        }

        protected ToolbarButtonData AddButton(string btnName, Action callback, bool left = true)
            => AddButton(new GUIContent(btnName), callback, left);
        
        protected ToolbarButtonData AddButton(GUIContent content, Action callback, bool left = true)
        {
            var data = new ToolbarButtonData
            {
                content = content,
                type = ElementType.Button,
                buttonCallback = callback
            };
            ((left) ? leftButtonDatas : rightButtonDatas).Add(data);
            return data;
        }

        protected ToolbarButtonData AddDropDownButton(GUIContent content, Action callback, bool left = true)
        {
            var data = new ToolbarButtonData
            {
                content = content,
                type = ElementType.DropDownButton,
                buttonCallback = callback
            };
            (left ? leftButtonDatas : rightButtonDatas).Add(data);
            return data;
        }

        protected ToolbarButtonData AddToggle(GUIContent content, bool defaultValue, Action<bool> callback,
            bool left = true)
        {
            var data = new ToolbarButtonData
            {
                content = content,
                type = ElementType.Toggle,
                value = defaultValue,
                toggleCallback = callback
            };
            (left ? leftButtonDatas : rightButtonDatas).Add(data);
            return data;
        }


        void DrawImGUIButtonList(List<ToolbarButtonData> buttons)
        {
            foreach (var button in buttons)
            {
                if (!button.visible)
                    continue;

                switch (button.type)
                {
                    case ElementType.Button:
                        if (GUILayout.Button(button.content, EditorStyles.toolbarButton) &&
                            button.buttonCallback != null)
                            button.buttonCallback();
                        break;
                    case ElementType.Toggle:
                        EditorGUI.BeginChangeCheck();
                        button.value = GUILayout.Toggle(button.value, button.content, EditorStyles.toggle);
                        if (EditorGUI.EndChangeCheck() && button.toggleCallback != null)
                            button.toggleCallback(button.value);
                        break;
                    case ElementType.DropDownButton:
                        if (EditorGUILayout.DropdownButton(button.content, FocusType.Passive,
                                EditorStyles.toolbarDropDown))
                            button.buttonCallback();
                        break;
                    case ElementType.Separator:
                        EditorGUILayout.Separator();
                        EditorGUILayout.Space(button.size);
                        break;
                    case ElementType.Custom:
                        button.customDrawFunction();
                        break;
                    case ElementType.FlexibleSpace:
                        GUILayout.FlexibleSpace();
                        break;
                }
            }
        }
        
        protected virtual void DrawImGUIToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            DrawImGUIButtonList(leftButtonDatas);

            GUILayout.FlexibleSpace();

            DrawImGUIButtonList(rightButtonDatas);

            GUILayout.EndHorizontal();
        }
    }
    
}
