using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

namespace utilities.ui
{
    public class DialogueDocController : BaseDocController
    {
        [SerializeField] TextAsset dialogueFile;

        [SerializeField] List<Dialogue> dialogueList;

        string dialogueTitle = "Tutorial";
        Dialogue currentDialogue;
        bool hasChoice = false;

        Label personName;
        Label dialogueText;
        VisualElement dialogueEnd;
        VisualElement choicePanel;

        private void Start()
        {
            LoadDialogue();
            ShowDoc(true);
        }

        private void Update()
        {
            if (!hasChoice && Input.GetKeyDown(KeyCode.Space))
            {
                ShowDialogue(currentDialogue.nextTitle);
            }
        }

        protected override void SetComponents()
        {
            personName = Root.Q<Label>("PersonName");
            dialogueText = Root.Q<Label>("DialogueText");
            dialogueEnd = Root.Q<VisualElement>("DialogueEnd");
            choicePanel = Root.Q<VisualElement>("Choices");
        }

        public override void ShowDoc(bool show, bool force = false)
        {
            ShowDialogue(dialogueTitle);
            base.ShowDoc(show, force);
        }

        private void ShowDialogue(string title)
        {
            if (title == null || title == string.Empty)
            {
                ShowDoc(false);
                return;
            }

            bool canGoForward = false;
            foreach (var dialogue in dialogueList) 
            {
                if (dialogue.title == title)
                { 
                    personName.text = dialogue.character;
                    dialogueText.text = dialogue.text;

                    if (dialogue.choices.Count > 0)
                    {
                        foreach (var choice in dialogue.choices) 
                        {
                            canGoForward = true;
                            Button button = new()
                            {
                                text = choice.text  
                            };

                            button.clicked += () => { ChoiceButton_clicked(button, choice); };

                            choicePanel.Add(button);
                        }
                        
                    }

                    hasChoice = canGoForward;
                    currentDialogue = dialogue;
                    break;    
                }
            }
        }

        private void ChoiceButton_clicked(Button button, Choice choice)
        {
            choicePanel.Clear();
            ShowDialogue(choice.nextDialogueTitle);
        }

        private void LoadDialogue()
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(dialogueFile.text);

            XmlNodeList dialogueNodes = xmlDoc.GetElementsByTagName("dialogue");

            foreach (XmlNode dialogueNode in dialogueNodes)
            {
                string dialogueId = dialogueNode.Attributes["id"].Value;

                XmlNode lineNode = dialogueNode.SelectSingleNode("line");

                string character = lineNode.SelectSingleNode("character")?.InnerText;
                string text = lineNode.SelectSingleNode("text")?.InnerXml; // Keep inner XML for tags like <highlight>
                string nextDialogueId = dialogueNode.SelectSingleNode("next")?.Attributes["next"].Value;

                XmlNodeList choiceNodes = lineNode.SelectNodes("choices/choice");

                List<Choice> choiceList = new();

                foreach (XmlNode choiceNode in choiceNodes)
                {
                    string choiceText = choiceNode.Attributes["text"]?.Value;
                    string nextDialogue = choiceNode.Attributes["next"]?.Value;
                    
                    Choice choice = new(choiceText,nextDialogue);
                    choiceList.Add(choice);
                }

                Dialogue dialogue = new(dialogueId, character, text, nextDialogueId, choiceList);
                dialogueList.Add(dialogue);
            }
        }
    }
}
