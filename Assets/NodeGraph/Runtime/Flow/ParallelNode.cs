using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    [NodeMenu("Nodes/Flow/Parallel", "并行")]
    public class ParallelNode : BaseNode
    {
        [SerializeField]
        private List<int> m_dynamicIds = new (){1, 2};
        
        private List<PortData> dynamicOutport = new()
        {
            new PortData("out_1"),
            new PortData("out_2"),
        };
        
        public override List<PortData> outputPortIds => dynamicOutport;
       
        protected override void OnImpulseInPort(string portName, FlowContext ctx)
        {
            if (portName.Equals(INPUT_PORT))
            {
                Active();
            }   
        }
        
        protected override bool OnTick(float dt, FlowContext ctx)
        {
            if (isActive)
            {
                var list = outputPortIds;
                for (int i = 0; i < list.Count; i++)
                {
                    var pName = list[i].name;
                    ImpulseOutPort(pName,ctx);
                }    
            }
            return isActive;
        }

#if UNITY_EDITOR
        public void AddOutPort()
        {
            var portId = m_dynamicIds.Count; 
            m_dynamicIds.Add(portId+1);
            RefreshOutPorts();
        }

        public void RemoveOutPort()
        {
            var count = m_dynamicIds.Count;
            if (count > 2)
            {
                m_dynamicIds.RemoveAt(count - 1);
                RefreshOutPorts();
            }
        }
#endif
        
        
        protected override void OnInit()
        {
           
        }

        public override void OnSerialize()
        {
           
        }

        public override void Deserialize()
        {
            RefreshOutPorts();
        }

        private void RefreshOutPorts()
        {
            dynamicOutport.Clear();
            var ports = m_dynamicIds;
            for (int i = 0; i < ports.Count; i++)
            {
                var p = ports[i];
                dynamicOutport.Add(new PortData(string.Format("out_{0}",p)));
            }
        }
    }
}

