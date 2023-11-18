using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class BaseFlow
    {
        private readonly List<BaseNode> m_ticks = new();
        private readonly HashSet<int> m_tickCheck = new();
        private readonly List<BaseNode> m_nextTurn = new();
        private readonly Dictionary<int, BaseNode> m_nodes = new();
        private readonly FlowContext m_context;
        private BaseNode m_enterNode;
        private bool m_done;

        public BaseFlow()
        {
            m_context = new FlowContext(this);
        }

        private void Reset()
        {
            m_done = false;
            m_tickCheck.Clear();
            m_ticks.Clear();
            m_nextTurn.Clear();
            foreach (var v in m_nodes)
            {
                v.Value.Init();
            }
        }

        private void MakeNodesDone()
        {
            foreach (var v in m_nodes)
            {
                v.Value.MarkDone();
            }
        }

        private void TryAddToTickList(BaseNode node)
        {
            if (!m_tickCheck.Contains(node.id))
            {
                m_tickCheck.Add(node.id);
                m_ticks.Add(node);
            }
        }

        private void TryRemoveFromCheck(int id)
        {
            if (m_tickCheck.Contains(id))
            {
                m_tickCheck.Remove(id);
            }
        }

        public void Init(List<BaseNode> nodes)
        {
            m_nodes.Clear();
            foreach (var n in nodes)
            {
                m_nodes[n.id] = n;
                if (n is EnterNode enter)
                {
                    m_enterNode = enter;
                }
            }
            
            Debug.Assert(m_enterNode!=null,"没有EnterNode节点");
            Reset();
        }

        public void Start()
        {
            if (m_enterNode == null) return;
            TryAddToTickList(m_enterNode);
        }

        public void Stop()
        {
            MakeNodesDone();
            m_done = true;
            OnFlowFinised();
        }

        public void Restart()
        {
            Reset();
            Start();
        }

        private static void MoveContent(List<BaseNode> from, List<BaseNode> to)
        {
            for (int i = 0, cnt = from.Count; i < cnt; ++i)
            {
                to.Add(from[i]);
            }
        }
        
        public void Tick(float dt)
        {
            var ticks = m_ticks;
            if (ticks.Count > 0)
            {
                var ctx = m_context;
                var nextTurn = m_nextTurn;
                nextTurn.Clear();
                for (var i = 0; i < ticks.Count;)
                {
                    ctx.Reset();
                    var n = ticks[i];
                    if (n.Tick(dt, ctx))
                    {
                        var last = ticks.Count - 1;
                        if (i != last)
                        {
                            TryRemoveFromCheck(n.id);
                            ticks[i] = ticks[last];
                        }
                        
                        var lastNode = ticks[last];
                        TryRemoveFromCheck(lastNode.id);
                        ticks.RemoveAt(last);
                    }
                    else
                    {
                        ++i;
                    }

                    MoveContent(ctx.activeNodes, nextTurn);
                }

                for (int i = 0, ncnt = nextTurn.Count; i < ncnt; ++i)
                {
                    var node = nextTurn[i];
                    TryAddToTickList(node);
                }

                if (ticks.Count == 0 && !m_done)
                {
                    Stop();
                }
            }
        }
        
        protected virtual void OnFlowFinised()
        {
        }
    }
}