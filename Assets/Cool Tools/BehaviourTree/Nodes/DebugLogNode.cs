using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class DebugLogNode : ActionNode
    {
        public NodeProperty<string> message;
        
        protected override void OnStart()
        {
            Debug.Log($"OnStart: {message.Value}");
        }

        protected override void OnStop()
        {
            Debug.Log($"OnStop: {message.Value}");
        }

        protected override State OnUpdate()
        {
            Debug.Log($"OnUpdate: {message.Value}");

            return State.Success;
        }
    }
}