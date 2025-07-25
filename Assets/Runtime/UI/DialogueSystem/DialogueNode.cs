using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace utilities.ui
{
    public abstract class DialogueNode
    {
        public enum DialogueNodeType
        { 
            LINE,
            CHOICE
        }

        public string nameId;
        public DialogueNodeType nodeType;
        public string text;
        public bool isEnd;

        public DialogueNode(string id, DialogueNodeType nodeType, string text)
        {
            nameId = id;
            this.nodeType = nodeType;
            this.text = text;
        }

        public DialogueNode(string id, DialogueNodeType nodeType)
        {
            nameId = id;
            this.nodeType = nodeType;
            text = string.Empty;
        }

        public abstract void SetChild(DialogueNode node);
    }

    public class DialogueLineNode : DialogueNode
    {
        string character; //TODO reworka in una struttura
        public List<DialogueNode> nextNodes;

        public DialogueLineNode(string id) : base(id, DialogueNodeType.LINE)
        {
            this.nextNodes = new();
        }

        public DialogueLineNode(string id, string character, string text) : base(id, DialogueNodeType.LINE, text)
        { 
            this.character = character;
            this.nextNodes = new();
        }

        public override void SetChild(DialogueNode node)
        {
            nextNodes.Add(node);
        }
    }

    public class DialogueChoiceNode : DialogueNode
    {
        public DialogueNode nextNode;
        public DialogueChoiceNode(string id, string text) : base(id, DialogueNodeType.CHOICE, text)
        {
        }

        public override void SetChild(DialogueNode node)
        {
            nextNode = node;
        }
    }
}
