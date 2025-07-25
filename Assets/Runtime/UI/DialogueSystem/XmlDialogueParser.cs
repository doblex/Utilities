using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace utilities.ui
{
    public class XmlDialogueParser
    {
        public static Dictionary<string, DialogueNode> LoadDialogueFromXml(TextAsset xmlAsset)
        {
            Dictionary<string, DialogueNode> nodes = new();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlAsset.text);

            XmlNodeList dialogueNodes = doc.SelectNodes("/dialogues/dialogue");

            // First pass: create nodes
            foreach (XmlNode dialogueNode in dialogueNodes)
            {
                string dialogueId = dialogueNode.Attributes["id"].Value;
                XmlNode lineNode = dialogueNode.SelectSingleNode("line");

                string character = lineNode.SelectSingleNode("character")?.InnerText ?? "";
                string text = ParseFormattedText(lineNode.SelectSingleNode("text"));

                DialogueLineNode node = new DialogueLineNode(dialogueId, character, text);
                nodes[dialogueId] = node;

                // Handle choices (if present)
                XmlNode choicesNode = lineNode.SelectSingleNode("choices");
                if (choicesNode != null)
                {
                    foreach (XmlNode choiceXml in choicesNode.SelectNodes("choice"))
                    {
                        string choiceText = choiceXml.Attributes["text"].Value;
                        string nextId = choiceXml.Attributes["next"].Value;

                        DialogueChoiceNode choiceNode = new DialogueChoiceNode($"{dialogueId}_choice_{choiceText}", choiceText);
                        node.SetChild(choiceNode); // Line node → Choice

                        // Temporarily store next link to resolve later
                        choiceNode.SetChild(new DialogueLineNode(nextId)); // Placeholder, resolved later
                        nodes[choiceNode.nameId] = choiceNode;
                    }
                }

                // Handle <next> for linear flow
                XmlNode nextNode = lineNode.SelectSingleNode("next");
                if (nextNode != null && nextNode.Attributes["next"] != null)
                {
                    string nextId = nextNode.Attributes["next"].Value;
                    node.SetChild(new DialogueLineNode(nextId)); // Placeholder, resolved later
                }
            }

            // Second pass: resolve all placeholder children by ID
            foreach (var node in nodes.Values)
            {
                if (node is DialogueLineNode lineNode)
                {
                    for (int i = 0; i < lineNode.nextNodes.Count; i++)
                    {
                        DialogueNode placeholder = lineNode.nextNodes[i];
                        if (nodes.TryGetValue(placeholder.nameId, out DialogueNode resolved))
                        {
                            lineNode.nextNodes[i] = resolved;
                        }
                    }
                }
                else if (node is DialogueChoiceNode choiceNode)
                {
                    DialogueNode placeholder = choiceNode.nextNode;
                    if (placeholder != null && nodes.TryGetValue(placeholder.nameId, out DialogueNode resolved))
                    {
                        choiceNode.nextNode = resolved;
                    }
                }
            }

            return nodes;
        }

        // Simple recursive parser for <highlight> tags (if you want custom formatting support)
        private static string ParseFormattedText(XmlNode textNode)
        {
            if (textNode == null) return "";

            string result = "";
            foreach (XmlNode node in textNode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Text)
                {
                    result += node.Value;
                }
                else if (node.Name == "highlight")
                {
                    string color = node.Attributes["color"]?.Value ?? "yellow";
                    result += $"<color={color}>{node.InnerText}</color>";
                }
            }

            return result;
        }
    }

}
