using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class WaitNode : ActionNode
    {
        [SerializeField, InspectorDisabled] private float elapsed;
        
        public NodeProperty<float> duration;

        private float startTime;

        protected override void OnStart()
        {
            elapsed = 0;
        }

        protected override void OnStop()
        {
            elapsed = 0f;
        }

        protected override State OnUpdate()
        {
            elapsed += Time.deltaTime;
            
            if (elapsed > duration.Value)
            {
                return State.Success;
            }

            return State.Running;
        }
    }
}