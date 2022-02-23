using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoolTools.BehaviourTree
{
    [Serializable]
    public class Variable
    {
        public Variable()
        {
                
        }

        ~Variable()
        {
            if(ArrayHolder != null)
                Object.DestroyImmediate(ArrayHolder);
        }
            
        [SerializeField] protected string name;
        [SerializeField] private string typeCaption;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string TypeCaption
        {
            get => typeCaption;
            set => typeCaption = value;
        }

        public string GUID { get; }

        public int IntValue;
        public string StringValue;
        public float FloatValue;
        public bool BoolValue;
        public Vector2 Vector2Value;
        public Vector3 Vector3Value;
        public Object ObjectRefValue;

        public List<int> IntegerList => ArrayHolder.IntegerList;
        public List<string> StringList => ArrayHolder.StringList;
        public List<float> FloatList => ArrayHolder.FloatList;
        public List<bool> BooleanList => ArrayHolder.BooleanList;
        public List<Vector2> Vector2List => ArrayHolder.Vector2List;
        public List<Vector3> Vector3List => ArrayHolder.Vector3List;
        public List<Object> ObjectList => ArrayHolder.ObjectList;

        public List<T> ObjectListAs<T>() => ObjectList.Cast<T>().ToList();

        public ArrayTypeValueHolder ArrayHolder;
    }
}