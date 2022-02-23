using System;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public enum PropertyValueType {Literal, Blackboard, Variable}

    [Serializable]
    public class NodeProperty
    {
        [SerializeField] private PropertyValueType valueType;

        public PropertyValueType ValueType
        {
            get => valueType;
            set => valueType = value;
        }
    }
    
    [Serializable]
    public class NodeProperty<T> : NodeProperty
    {
        public NodeProperty()
        {
                
        }
        
        public T Value;
    }
    
}