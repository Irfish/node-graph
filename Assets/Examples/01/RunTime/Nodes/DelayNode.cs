using System;
using System.Collections.Generic;
using NodeGraph;
using UnityEngine;

namespace Example01
{
    [Serializable]
    [NodeMenu("Nodes/Test/Delay", "测试延时")]
    public class DelayNode : BaseNode
    {
        [SerializeField]
        private float delayTime = 3;

        private float m_time;

        public float delay => delayTime;

        public override List<PortData> outputPortIds => new() { new PortData(OUTPUT_PORT,true) };
        
        protected override void OnImpulseInPort(string portName, FlowContext ctx)
        {
            if (portName.Equals(INPUT_PORT))
            {
                Active();
                Debug.Log(string.Format("delay node active:{0}",id));
            }
        }

        protected override bool OnTick(float dt, FlowContext ctx)
        {
            if (isActive)
            {
                m_time+=dt;
                if (m_time >= delay)
                {
                    Debug.Log(string.Format("delay node done:{0}",id));
                    ImpulseOutPort(OUTPUT_PORT,ctx);
                    return true;
                }
            }
            return false;
        }

        protected override void OnInit()
        {
            m_time=0;
        }

        public override void OnSerialize()
        {
        }

        public override void Deserialize()
        {
        }

#if UNITY_EDITOR
        public void ResetDelay(float value)
        {
            delayTime = value;
        }
#endif
    }
}