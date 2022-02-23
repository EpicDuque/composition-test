using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoolTools.Graphs.Dialogue
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> Nodelinks = new();
        public List<DialogueNodeData> DialoguesNodeData = new();
    }
}
