using System;
using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public abstract class Node : ScriptableObject
    {
        public enum State {Running, Failure, Success, Idle}

        [Header("Node State")]
        [SerializeField, InspectorDisabled] private BehaviourTree tree;
        [InspectorDisabled] public State state = State.Idle;
        [InspectorDisabled] public bool started;

        [Space(10f)] 
        [TextArea] public string description;
        
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;

        public BehaviourTree Tree
        {
            get => tree;
            set => tree = value;
        }

        public State Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            state = OnUpdate();

            if (state is State.Failure or State.Success)
            {
                OnStop();
                started = false;
            }

            return state;
        }

        public virtual void Restart()
        {
            started = false;
            state = State.Running;
        }

        public virtual void Stop()
        {
            OnStop();
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
    }
}