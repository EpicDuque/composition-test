using CoolTools.BehaviourTree;

namespace CoolTools.BehaviourTree
{
    public class SequencerNode : CompositeNode
    {
        private int current = 0;

        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            while (CanEvaluateNextChild(Children[current].Update()))
            {
                current++;
                if (current == Children.Count)
                    return State.Success;
            }

            return EvaluationLoopState();
        }

        protected virtual State EvaluationLoopState() => Children[current].state;

        protected virtual bool CanEvaluateNextChild(State s) => s == State.Success;
    }
}