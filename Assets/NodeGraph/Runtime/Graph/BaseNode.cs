using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public abstract class BaseNode : ISerializationCallbackReceiver
    {
        protected const string INPUT_PORT = "in";
        protected const string OUTPUT_PORT = "out";

        public int id;
        public Rect position;
        [NonSerialized] public string GUID;
        [NonSerialized] protected BaseGraph graph;
        [NonSerialized] public List<NodePort> inputPorts = new();
        [NonSerialized] public List<NodePort> outputPorts = new();
        [NonSerialized] public string customName = string.Empty;
        private bool m_active;
        private bool m_done;

        public virtual List<PortData> inputPortIds => new() { new PortData(INPUT_PORT) };

        public virtual List<PortData> outputPortIds => new() { new PortData(OUTPUT_PORT) };

        public bool isDone => m_done;

        public bool isActive => m_active;

        public virtual string name
        {
            get
            {
                if (string.IsNullOrEmpty(customName))
                {
                    return GetType().Name;
                }

                if (id > 0)
                {
                    return string.Format("{0} 【<color=#0088ff>{1}</color>】", customName, id);
                }
                else
                {
                    return string.Format("{0} 【<color=#00ff00>New</color>】", customName);
                }
            }
        }

        public void InitWithGraph(BaseGraph g)
        {
            m_done = false;
            m_active = false;
            graph = g;
            InitPorts();
        }

        private void InitPorts()
        {
            var inputPortIdList = inputPortIds;
            for (int i = 0; i < inputPortIdList.Count; i++)
            {
                AddPort(NodePortType.Input, inputPortIdList[i]);
            }

            var outputPortIdList = outputPortIds;
            for (int i = 0; i < outputPortIdList.Count; i++)
            {
                AddPort(NodePortType.Output, outputPortIdList[i]);
            }
        }

        private NodePort AddPort(NodePortType portType, PortData data)
        {
            switch (portType)
            {
                case NodePortType.Input:
                    var inputPort = new NodePort(this, data);
                    inputPorts.Add(inputPort);
                    return inputPort;
                case NodePortType.Output:
                    var outputPort = new NodePort(this, data);
                    outputPorts.Add(outputPort);
                    return outputPort;
            }

            return null;
        }

        public NodePort GetPort(NodePortType portType, string portName)
        {
            switch (portType)
            {
                case NodePortType.Input:
                    foreach (var port in inputPorts)
                    {
                        if (port.portName.Equals(portName))
                            return port;
                    }

                    return null;
                case NodePortType.Output:
                    foreach (var port in outputPorts)
                    {
                        if (port.portName.Equals(portName))
                            return port;
                    }

                    return null;
            }

            return null;
        }
            
        public void CollectConnectionNodes(List<BaseNode> nodes)
        {
            foreach (var port in outputPorts)
            {
                port.CollectConnectionNodes(nodes);
            }
        }

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize()
        {
            OnSerialize();
        }

        public void OnAfterDeserialize()
        {
            Deserialize();
        }

        #endregion

        public void Init()
        {
            m_done = false;
            m_active = false;
            OnInit();
#if UNITY_EDITOR
            onNodeInit?.Invoke();
#endif
        }

        public void MarkDone()
        {
            m_done = true;
            OnFinished();
#if UNITY_EDITOR
            onNodeFinished?.Invoke();
#endif
        }

        protected void Active()
        {
            m_active = true;
#if UNITY_EDITOR
            onNodeActive?.Invoke();
#endif
        }

        public bool Tick(float dt, FlowContext ctx)
        {
            if (isDone)
            {
                return true;
            }

            bool finished = false;
            try
            {
                finished = OnTick(dt, ctx);
#if UNITY_EDITOR
                onNodeTick?.Invoke(dt);
#endif
                if (finished)
                {
                    MarkDone();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return finished;
        }

        /// <summary>
        /// 传输input信号
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="ctx"></param>
        public void ImpulseInPort(string portName, FlowContext ctx)
        {
            try
            {
                if (!isDone)
                {
                    OnImpulseInPort(portName, ctx);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 传输output信号
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="ctx"></param>
        protected void ImpulseOutPort(string portName, FlowContext ctx)
        {
            try
            {
                var outPort = GetPort(NodePortType.Output, portName);
                if (outPort != null)
                {
                    outPort.Impulse(ctx);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected virtual bool OnTick(float dt, FlowContext ctx)
        {
            return true;
        }

        protected virtual void OnFinished()
        {
        }

        protected virtual void OnImpulseInPort(string portName, FlowContext ctx)
        {
            foreach (var port in inputPorts)
            {
                if (port.portName.Equals(portName))
                {
                    Active();
                }
            }
        }

        protected abstract void OnInit();
        public abstract void OnSerialize();
        public abstract void Deserialize();

#if UNITY_EDITOR
        public Action onNodeInit;
        public Action onNodeActive;
        public Action onNodeFinished;
        public Action<float> onNodeTick;
#endif
    }
}