using NodeGraph;
using UnityEngine;

namespace Example01
{
    [NodeMenu("Nodes/Test/PrintNode", "测试节点01")]
    public class PrintNode : BaseNode
    {
        protected override void OnImpulseInPort(string portName, FlowContext ctx)
        {
            if (portName.Equals(INPUT_PORT))
            {
                Active();
                Debug.Log(string.Format("print node active:{0}", id));
            }
        }

        protected override bool OnTick(float dt, FlowContext ctx)
        {
            if (isActive)
            {
                Debug.Log(string.Format("print node :{0}", id));
                ImpulseOutPort(OUTPUT_PORT, ctx);
                return true;
            }

            return false;
        }

        protected override void OnInit()
        {
        }

        public override void OnSerialize()
        {
        }

        public override void Deserialize()
        {
        }
    }
}