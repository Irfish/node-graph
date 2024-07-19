using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using GraphNode = UnityEditor.Experimental.GraphView.Node;
using GraphPort = UnityEditor.Experimental.GraphView.Port;

namespace NodeGraph.Editor
{
    public class NodeView<V, T> : GraphNode where T : BaseNode where V : NodeView<V, T>
    {
        protected static readonly Color node_color_normal = Color.gray;
        protected static readonly Color node_color_active = Color.green;
        protected static readonly Color node_color_finished = Color.red;
        protected static readonly Color port_color_in = Color.cyan*0.5f;
        protected static readonly Color port_color_out = Color.yellow*0.5f;
        protected static readonly Color port_color_finished = Color.red;
        public T target;
        private BaseGraphView graphView;
        protected VisualElement controlsContainer;
        protected readonly Dictionary<string, GraphPort> m_inputPorts = new();
        protected readonly Dictionary<string, GraphPort> m_outputPorts = new();
        private float tickTime;
        
        protected virtual void Init(BaseGraphView gView, T node)
        {
            target = node;
            graphView = gView;
            title = node.name;
            tickTime = 0;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SrcDefine.baseNodeStylePath);
            styleSheets.Add(styleSheet);
            controlsContainer = new VisualElement { name = "controls" };
            controlsContainer.AddToClassList("NodeControls");
            mainContainer.Add(controlsContainer);

            node.onNodeInit = OnNodeInit;
            node.onNodeActive = OnNodeActive;
            node.onNodeFinished = OnNodeFinished;
            node.onNodeTick = OnNodeTick;
            node.onImpulseInPort = OnImpulseInPort;
            node.onImpulseOutPort = OnImpulseOutPort;

            SetNodeHeadColor(node_color_normal);
        }

        private void OnNodeInit()
        {
            tickTime = 0;
            
            SetNodeHeadColor(node_color_normal);

            if (graphView.editorModel)
            {
                SetPortsEnabled(true);
            }
        }

        private void OnNodeActive()
        {
            SetNodeHeadColor(node_color_active);
            SetPortsEnabled(false);
        }

        protected void SetPortsEnabled(bool enabled)
        {
            foreach (var port in m_inputPorts)
            {
                foreach (var edge in port.Value.connections)
                {
                    edge.SetEnabled(enabled);
                }

                port.Value.SetEnabled(enabled);
            }

            foreach (var port in m_outputPorts)
            {
                foreach (var edge in port.Value.connections)
                {
                    edge.SetEnabled(enabled);
                }

                port.Value.SetEnabled(enabled);
            }

            this.SetEnabled(enabled);
        }

        private void OnNodeFinished()
        {
            SetNodeHeadColor(node_color_finished);
            foreach (var port in m_inputPorts)
            {
                port.Value.portColor = port_color_finished;
            }  
            foreach (var port in m_outputPorts)
            {
                port.Value.portColor =  port_color_finished;
            }  
        }

        private void TickActivedColor()
        {
            var change = (tickTime - (int)tickTime) <= 0.5f;
            SetNodeHeadColor(node_color_active*(change?1:0.5f));
        }
        
        private void OnImpulseInPort(string portName)
        {
            if (m_inputPorts.TryGetValue(portName, out var port))
            {
                port.portColor = port_color_finished;
            }
        }
        
        private void OnImpulseOutPort(string portName)
        {
            if (m_outputPorts.TryGetValue(portName, out var port))
            {
                port.portColor = port_color_finished;
            }
        }
        
        private void OnNodeTick(float dt)
        {
            tickTime += dt;
            TickActivedColor();
        }

        protected virtual void SetNodeHeadColor(Color color)
        {
            titleContainer.style.borderTopColor = new StyleColor(color);
            titleContainer.style.borderTopWidth = new StyleFloat(4f);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            target.position = newPos;
        }

        public sealed override string title
        {
            set => base.title = value;
            get => base.title;
        }

        public static V CreateNodeView(BaseGraphView graphView, T node)
        {
            var viewType = NodeTypes.GetViewType(node.GetType());
            if (viewType != null)
            {
                var menu = NodeTypes.GetMenu(viewType);
                if (menu != null)
                {
                    var view = Activator.CreateInstance(viewType) as V;
                    if (view != null)
                    {
                        //node.GUID = Guid.NewGuid().ToString();
                        node.customName = menu.nodeTitle;
                        view.Init(graphView, node);
                        return view;
                    }
                }
            }

            return null;
        }

        public static V CreateNodeView(BaseGraphView graphView, NodeMenu menu)
        {
            var view = Activator.CreateInstance(menu.viewType) as V;
            var node = Activator.CreateInstance(menu.nodeType) as T;
            if (view != null && node != null)
            {
                node.customName = menu.nodeTitle;
                node.id = BaseGraphView.GenNodeId();
                node.Reset();
                view.Init(graphView, node);
                return view;
            }

            return null;
        }
    }

    public abstract class BaseNodeView : NodeView<BaseNodeView, BaseNode>
    {
        protected sealed override void Init(BaseGraphView gView, BaseNode node)
        {
            base.Init(gView, node);
            InitPorts();
            OnInit();

            SetPortsEnabled(!node.isActive);
            if (node.isActive)
            {
                SetNodeHeadColor(node_color_active);
            }

            if (node.isDone)
            {
                SetNodeHeadColor(node_color_finished);
            }
        }

        private void InitPorts()
        {
            var inputPortIds = target.inputPortIds;
            for (int i = 0; i < inputPortIds.Count; i++)
            {
                var port = inputPortIds[i];
                AddPort(Direction.Input, port);
                if (target.m_inPortImpulsed.Contains(port.name))
                {
                    m_inputPorts[port.name].portColor = port_color_finished;
                }
            }

            var outputPortIds = target.outputPortIds;
            for (int i = 0; i < outputPortIds.Count; i++)
            {
                var port = outputPortIds[i];
                AddPort(Direction.Output, port);
                if (target.m_outPortImpulsed.Contains(port.name))
                {
                    m_outputPorts[port.name].portColor = port_color_finished;
                }
            }
        }

        private void AddPort(Direction type, PortData data)
        {
            var capacity = data.multiLink ? GraphPort.Capacity.Multi : GraphPort.Capacity.Single;
            var vertical = data.vertical ? Orientation.Vertical : Orientation.Horizontal;
            switch (type)
            {
                case Direction.Input:
                    var inPort = InstantiatePort(vertical, type, capacity, typeof(bool));
                    inPort.portName = data.name;
                    inPort.portColor = port_color_in;
                    inputContainer.Add(inPort);
                    m_inputPorts[data.name] = inPort;
                    break;
                case Direction.Output:
                    var outPort = InstantiatePort(vertical, type, capacity, typeof(bool));
                    outPort.portName = data.name;
                    outPort.portColor = port_color_out;
                    outputContainer.Add(outPort);
                    m_outputPorts[data.name] = outPort;
                    break;
            }
        }

        public GraphPort GetPort(Direction type, string portName)
        {
            switch (type)
            {
                case Direction.Input:
                    m_inputPorts.TryGetValue(portName, out var inputPort);
                    return inputPort;
                case Direction.Output:
                    m_outputPorts.TryGetValue(portName, out var outputPort);
                    return outputPort;
            }

            return null;
        }

        protected abstract void OnInit();
        
        protected IntegerField DrawIntegerField(string text,int value,EventCallback<ChangeEvent<int>> valueChangedCallback)
        {
            var field = new IntegerField(text);
            field.labelElement.style.minWidth = 50;
            field.value = value;    
            field.RegisterValueChangedCallback(valueChangedCallback);
            controlsContainer.Add(field);
            return field;
        }
        
        protected FloatField DrawFloatField(string text,float value,EventCallback<ChangeEvent<float>> valueChangedCallback)
        {
            var field = new FloatField(text);
            field.labelElement.style.minWidth = 50;
            field.value = value;    
            field.RegisterValueChangedCallback(valueChangedCallback);
            controlsContainer.Add(field);
            return field;
        }
        
        protected Toggle DrawToggle(string text,bool value,EventCallback<ChangeEvent<bool>> valueChangedCallback)
        {
            var field = new Toggle(text);
            field.labelElement.style.minWidth = 50;
            field.value = value;    
            field.RegisterValueChangedCallback(valueChangedCallback);
            controlsContainer.Add(field);
            return field;
        }
        
        protected Button DrawButton(string text,Action action)
        {
            var btn = new Button(action)
            {
                text = text
            };
            controlsContainer.Add(btn);
            return btn;
        }
    }
}