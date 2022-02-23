using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public abstract class DecoratorNode : Node
    {
        [HideInInspector] public Node Child;

        public override Node Clone()
        {
            var node = base.Clone() as DecoratorNode;
            
            node.Child = Child.Clone();

            return node;
        }

        public override void Restart()
        {
            base.Restart();

            if (Child == null) return;
            
            Child.Restart();
        }

        // protected override void OnStop()
        // {
        //     if (state == State.Failure)
        //         Child.state = State.Idle;
        // }
    }
}