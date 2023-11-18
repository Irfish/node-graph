using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    [NodeMenu("Nodes/Flow/Enter","入口")]
    public class EnterNode : BaseNode
    {
        /// <summary>
        /// 没有输入端口
        /// </summary>
        public override List<PortData> inputPortIds => new();
        
        protected override bool OnTick(float dt, FlowContext ctx)
        {
            Debug.Log("Start");
            ImpulseOutPort(OUTPUT_PORT,ctx);
            return true;
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

