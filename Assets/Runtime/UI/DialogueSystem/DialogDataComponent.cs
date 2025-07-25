using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using ReadOnlyAttribute = utilities.general.attributes.ReadOnlyAttribute;

namespace utilities.ui
{
    public class DialogDataComponent : MonoBehaviour
    {
        [SerializeField] TextAsset dialogueFile;

        Dictionary<string, DialogueNode> DialogueNodes;

        DialogueNode currentNode = null;

        [SerializeField] string nextTitleID = "Prova 1";

        private void Awake()
        {
            LoadDialogueList();
            currentNode = DialogueNodes[nextTitleID];
        }

        //public DialogueNode GetNextNode()

        void ShowNode(DialogueNode node)
        {
            if (node is DialogueLineNode line)
            {
                Debug.Log($"[{line.nameId}] {line.text}");
                foreach (var next in line.nextNodes)
                {
                    if (next is DialogueChoiceNode choice)
                    {
                        Debug.Log($"  -> Choice: {choice.text}");
                    }
                }
            }
            else if (node is DialogueChoiceNode choice)
            {
                Debug.Log($"Choice: {choice.text}");
            }
        }

        private void LoadDialogueList()
        {
            DialogueNodes = XmlDialogueParser.LoadDialogueFromXml(dialogueFile);
        }
    }
}
