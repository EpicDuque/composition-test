using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoolTools.BehaviourTree
{
    public class Blackboard : MonoBehaviour
    {
        // [SerializeField] private string title = "Title";
        // [SerializeField, Multiline] private string description;
        [SerializeField] private List<Variable> variables = new ();
    
        // public string Title => title;
        //
        // public string Description => description;
    
        public List<Variable> Variables => variables;

        public Variable GetVariable(string varName)
        {
            return Variables.FirstOrDefault(v => v.Name.Equals(varName));
        }

        public T GetRefValue<T>(string varName) where T : Object
        {
            var variable = GetVariable(varName);

            return variable?.ObjectRefValue as T;
        }
        
    }
}