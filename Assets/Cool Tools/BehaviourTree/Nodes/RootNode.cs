using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class RootNode : Node
    {
        [HideInInspector] public Node child;
        
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            return child.Update();
        }

        public override Node Clone()
        {
            var node = base.Clone() as RootNode;
            node.child = child.Clone();

            return node;
        }

        public override void Restart()
        {
            base.Restart();
            if (child == null) return;
            
            child.Restart();
        }
    }
}