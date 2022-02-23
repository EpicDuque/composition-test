using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoolTools.BehaviourTree
{
    public class ArrayTypeValueHolder : MonoBehaviour
    {
        
        public enum ArrayType
        {
            Integer, Float, Boolean, String, Vector2, Vector3, Object
        }

        [SerializeField, HideInInspector] private ArrayType type;
        public bool Hide;

        public ArrayType Type
        {
            get => type;
            set => type = value;
        }

        public List<int> IntegerList;
        public List<bool> BooleanList;
        public List<float> FloatList;
        public List<string> StringList;
        public List<Vector2> Vector2List;
        public List<Vector3> Vector3List;
        public List<Object> ObjectList;
        
        private void OnValidate()
        {
            
        }

        public IList GetTypeArray()
        {
            switch (type)
            {
                case ArrayType.Integer:
                    return IntegerList;
                case ArrayType.Float:
                    return FloatList;
                case ArrayType.Boolean:
                    return BooleanList;
                case ArrayType.String:
                    return StringList;
                case ArrayType.Vector2:
                    return Vector2List;
                case ArrayType.Vector3:
                    return Vector3List;
                case ArrayType.Object:
                    return ObjectList;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}