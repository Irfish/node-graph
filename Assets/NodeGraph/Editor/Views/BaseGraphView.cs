using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using GraphPort = UnityEditor.Experimental.GraphView.Port;

namespace NodeGraph.Editor
{
    public sealed class BaseGraphView : GraphView
    {
        
        public IGraphSerializer graphSerializer;

        private SearchNodeWindow m_searchWindowProvider;

        private Vector2 m_clickPosition;
        public event Action initEvents;

        private readonly Dictionary<int, BaseNodeView> m_nodeViewCache = new();

        private bool m_editorModel = true;

        private static int m_nodeMaxID;

        public static int GenNodeId()
        {
            return ++m_nodeMaxID;
        }

        public bool editorModel
        {
            set
            {
                if (m_editorModel != value)
                {
                    SetElementsEnabled(value);
                }

                m_editorModel = value;
            }
            get => m_editorModel;
        }

        public BaseGraphView(EditorWindow window)
        {
            SetBackGroud();
            InitSearchWindow();

            nodeCreationRequest = NodeCreationRequest;
            graphViewChanged += OnGraphViewChanged;
            deleteSelection += OnDeleteSelection;

            RegisterCallback<KeyDownEvent>(OnKeyDownCallback);

            IniteManipulators();
            SetupZoom(0.05f, 2f);
            this.StretchToParentSize();
        }

        private void IniteManipulators()
        {
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void OnKeyDownCallback(KeyDownEvent e)
        {
            if (e.keyCode == KeyCode.S && e.shiftKey && e.ctrlKey)
            {
                SaveGraphToDiskAsNew();
                e.StopPropagation();
            }
            else if (e.keyCode == KeyCode.S && e.ctrlKey)
            {
                SaveGraphToDisk();
                e.StopPropagation();
            }
        }

        private void SetBackGroud()
        {
            Insert(0, new GridBackground());
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(SrcDefine.graphBackGroundPath);
            styleSheets.Add(styleSheet);
        }

        private void InitSearchWindow()
        {
            m_searchWindowProvider = ScriptableObject.CreateInstance<SearchNodeWindow>();
            m_searchWindowProvider.onSelectMenuEntryHandler = OnSelectMenuEntry;
        }

        private void OnDeleteSelection(string optName, AskUser askUser)
        {
            this.DeleteSelection();
        }

        public override EventPropagation DeleteSelection()
        {
            if (!editorModel) return EventPropagation.Continue;
            return base.DeleteSelection();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            var createEdges = graphviewchange.edgesToCreate;
            if (createEdges != null && createEdges.Count > 0)
            {
                foreach (var edge in createEdges)
                {
                    OnEdgeAdd(edge);
                }
            }

            var removeElements = graphviewchange.elementsToRemove;
            if (removeElements != null && removeElements.Count > 0)
            {
                foreach (var element in removeElements)
                {
                    if (element is Edge edge)
                    {
                        OnEdgeRemove(edge);
                    }

                    if (element is BaseNodeView nodeView)
                    {
                        OnNodeViewRemove(nodeView);
                    }
                }
            }

            return graphviewchange;
        }

        private void OnEdgeAdd(Edge edge)
        {
            Debug.Log("添加 edge");
            var input = edge.input;
            var output = edge.output;
            if (input != null && output != null)
            {
                if (input.node is BaseNodeView inputView && output.node is BaseNodeView outputView)
                {
                    var inputNode = inputView.target;
                    var outputNode = outputView.target;
                    var inputPort = inputNode.GetPort(NodePortType.Input, input.portName);
                    var outputPort = outputNode.GetPort(NodePortType.Output, output.portName);
                    if (inputPort != null && outputPort != null)
                    {
                        inputPort.LinkTo(outputPort);
                        outputPort.LinkTo(inputPort);
                    }
                }
            }
        }

        private void OnEdgeRemove(Edge edge)
        {
            Debug.Log("remove edge");
            var input = edge.input;
            var output = edge.output;
            if (input != null && output != null)
            {
                if (input.node is BaseNodeView inputView && output.node is BaseNodeView outputView)
                {
                    var inputNode = inputView.target;
                    var outputNode = outputView.target;
                    var inputPort = inputNode.GetPort(NodePortType.Input, input.portName);
                    var outputPort = outputNode.GetPort(NodePortType.Output, output.portName);
                    if (inputPort != null && outputPort != null)
                    {
                        inputPort.Delink(outputPort);
                        outputPort.Delink(inputPort);
                    }
                }
            }
        }

        private void OnNodeViewRemove(BaseNodeView nodeView)
        {
            var node = nodeView.target;
            if (node != null)
            {
                if (m_nodeViewCache.ContainsKey(node.id))
                {
                    m_nodeViewCache.Remove(node.id);
                }
            }
        }

        private void OnNodeViewAdd(BaseNodeView nodeView)
        {
            var node = nodeView.target;
            if (node != null)
            {
                m_nodeViewCache[node.id] = nodeView;
            }
        }

        private void SetElementsEnabled(bool anable)
        {
            var portList = ports.ToList();
            foreach (var port in portList)
            {
                if (anable)
                {
                    if (port.node is BaseNodeView nodeView)
                    {
                        var n = nodeView.target;
                        if (n.isActive)
                        {
                            port.SetEnabled(false);
                            continue;
                        }
                    }
                }

                port.SetEnabled(anable);
            }

            var edgeList = edges.ToList();
            foreach (var edge in edgeList)
            {
                if (anable)
                {
                    if (edge.input.node is BaseNodeView inputNodeView)
                    {
                        var n = inputNodeView.target;
                        if (n.isActive)
                        {
                            edge.SetEnabled(false);
                            continue;
                        }
                    }

                    if (edge.output.node is BaseNodeView outNodeView)
                    {
                        var n = outNodeView.target;
                        if (n.isActive)
                        {
                            edge.SetEnabled(false);
                            continue;
                        }
                    }
                }

                edge.SetEnabled(anable);
            }
        }

        public void ResetPositionAndZoom()
        {
            UpdateViewTransform(Vector3.zero, Vector3.one);
        }

        private void NodeCreationRequest(NodeCreationContext context)
        {
            if (!editorModel) return;

            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_searchWindowProvider);
        }

        private bool OnSelectMenuEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is NodeMenu menu)
            {
                return AddNode(menu);
            }

            return false;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            m_clickPosition = contentViewContainer.WorldToLocal(evt.mousePosition);

            if (!editorModel) return;

            var list = NodeTypes.GetNodeMenus();
            foreach (var menu in list)
            {
                evt.menu.AppendAction(menu.menuPath, (x) => { AddNode(menu); });
            }
        }

        public override List<GraphPort> GetCompatiblePorts(GraphPort startPort, NodeAdapter nodeAdapter)
        {
            var start = startPort;
            var list = ports.ToList().Where(endport =>
                {
                    return endport.direction != start.direction &&
                           endport.node != start.node &&
                           endport.portType == start.portType;
                }
            ).ToList();
            return list;
        }

        private bool AddNode(NodeMenu menu)
        {
            var editor = BaseNodeView.CreateNodeView(this, menu);
            if (editor != null)
            {
                var rect = editor.GetPosition();
                rect.position = m_clickPosition;
                editor.SetPosition(rect);
                AddElement(editor);
                OnNodeViewAdd(editor);
                return true;
            }

            return false;
        }

        private BaseNodeView AddNode(BaseNode node)
        {
            var editor = BaseNodeView.CreateNodeView(this, node);
            if (editor != null)
            {
                editor.SetPosition(node.position);
                AddElement(editor);
                OnNodeViewAdd(editor);
                return editor;
            }

            return null;
        }


        public void Init(IGraphSerializer g)
        {
            graphSerializer = g;
            ClearView();
            InitView();
            OnInit();
        }

        private void OnInit()
        {
            initEvents?.Invoke();
        }

        private void ClearView()
        {
            m_nodeViewCache.Clear();
            var nodeList = nodes.ToList();
            foreach (var n in nodeList)
            {
                RemoveElement(n);
            }

            var edgeList = edges.ToList();
            foreach (var e in edgeList)
            {
                RemoveElement(e);
            }
        }


        private void InitView()
        {
            m_nodeViewCache.Clear();
            var graphNodes = graphSerializer.graph.nodes;
            graphNodes.RemoveAll(n => n == null);
            m_nodeMaxID = 0;

            foreach (var node in graphNodes)
            {
                var view = AddNode(node);
                if (view != null)
                {
                    m_nodeViewCache[node.id] = view;
                    if (m_nodeMaxID < node.id)
                    {
                        m_nodeMaxID = node.id;
                    }
                }
            }

            var graphConnections = graphSerializer.graph.connections;
            graphConnections.RemoveAll(n => n == null);
            foreach (var e in graphConnections)
            {
                m_nodeViewCache.TryGetValue(e.inputNodeId, out var inputNode);
                m_nodeViewCache.TryGetValue(e.outputNodeId, out var outputNode);
                if (inputNode != null && outputNode != null)
                {
                    var inputPort = inputNode.GetPort(Direction.Input, e.inputPortName);
                    var outputPort = outputNode.GetPort(Direction.Output, e.outputPortName);
                    if (inputPort != null && outputPort != null)
                    {
                        var edge = new Edge();
                        edge.input = inputPort;
                        edge.output = outputPort;
                        ConnectView(edge);
                    }
                }
            }
        }

        private void AddNodeToScriptableGraph(BaseNodeView editor)
        {
            var node = editor.target;
            if (node != null)
            {
                graphSerializer.graph.AddNode(node);
            }
        }

        private PortConnection CreateEdgeData(Edge e)
        {
            var inputPortView = e.input;
            var outputPortView = e.output;
            if (inputPortView == null || outputPortView == null) return null;

            var inputNodeView = inputPortView.node as BaseNodeView;
            var outputNodeView = outputPortView.node as BaseNodeView;
            if (inputNodeView == null || outputNodeView == null) return null;

            var date = new PortConnection()
            {
                inputPortName = inputPortView.portName,
                outputPortName = outputPortView.portName,
                inputNodeId = inputNodeView.target.id,
                outputNodeId = outputNodeView.target.id,
            };
            return date;
        }

        private void AddEdgeToScriptableGraph(Edge e)
        {
            var data = CreateEdgeData(e);
            if (data != null)
            {
                graphSerializer.graph.AddConnection(data);
            }
        }

        private bool CheckEdge(Edge e)
        {
            if (e.input == null || e.output == null)
                return false;
            var inputPortView = e.input;
            var outputPortView = e.output;
            var inputNodeView = inputPortView.node as BaseNodeView;
            var outputNodeView = outputPortView.node as BaseNodeView;
            if (inputNodeView == null || outputNodeView == null)
            {
                Debug.LogError("Connect aborted !");
                return false;
            }

            return true;
        }

        private bool ConnectView(Edge edge)
        {
            if (edge == null) return false;

            if (!CheckEdge(edge))
                return false;

            var inputPortView = edge.input;
            var outputPortView = edge.output;
            var inputNodeView = inputPortView.node as BaseNodeView;
            var outputNodeView = outputPortView.node as BaseNodeView;

            if (inputNodeView == null || outputNodeView == null)
            {
                return false;
            }

            AddElement(edge);

            edge.input.Connect(edge);
            edge.output.Connect(edge);

            inputNodeView.RefreshPorts();
            outputNodeView.RefreshPorts();

            schedule.Execute(() => { edge.UpdateEdgeControl(); }).ExecuteLater(1);

            return true;
        }

        public void RefreshGraph()
        {
            ResetGraphData();
            ClearView();
            InitView();
        }

        private void ResetGraphData()
        {
            graphSerializer.graph.Reset();

            var nodeList = nodes.ToList();
            for (var i = 0; i < nodeList.Count; i++)
            {
                var n = nodeList[i];
                if (n is BaseNodeView editor)
                {
                    editor.target.id = i + 1;
                    AddNodeToScriptableGraph(editor);
                }
            }

            m_nodeMaxID = nodeList.Count + 1;

            var list = edges.ToList();
            foreach (var e in list)
            {
                AddEdgeToScriptableGraph(e);
            }
        }

        public void SaveGraphToDisk()
        {
            ResetGraphData();
            FileUtils.SaveGraph(graphSerializer);
        }

        public void SaveGraphToDiskAsNew()
        {
            ResetGraphData();
            FileUtils.SaveGraphAsNew(graphSerializer);
        }
    }
}