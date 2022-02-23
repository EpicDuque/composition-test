using System.Collections.Generic;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public abstract class CompositeNode : Node
    {
        [HideInInspector] public List<Node> Children = new List<Node>();
        
        public override Node Clone()
        {
            var node = base.Clone() as CompositeNode;
            node.Children = new List<Node>(Children.ConvertAll(n => n.Clone()));

            return node;
        }

        public override void Restart()
        {
            base.Restart();
            
            Children.ForEach(c => c.Restart());
        }

        protected override void OnStop()
        {
            if(state == State.Failure)
                Children.ForEach(c => c.state = State.Idle);
        }
    }
}