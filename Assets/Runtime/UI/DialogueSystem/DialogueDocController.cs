using UnityEngine;
using UnityEngine.UIElements;

namespace utilities.ui
{
    public class DialogueDocController : BaseDocController
    {
        public static DialogueDocController Instance { get; private set; }

        Dialogue currentDialogueNode;
        bool hasChoice = false;

        Label personName;
        Label dialogueText;
        VisualElement dialogueEnd;
        VisualElement choicePanel;

        protected override void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            base.Awake();
        }

        private void Update()
        {
            if (!hasChoice && Input.GetKeyDown(KeyCode.Space))
            {
                ShowDialogue(currentDialogueNode.nextTitle);
            }
        }

        protected override void SetComponents()
        {
            personName = Root.Q<Label>("PersonName");
            dialogueText = Root.Q<Label>("DialogueText");
            dialogueEnd = Root.Q<VisualElement>("DialogueEnd");
            choicePanel = Root.Q<VisualElement>("Choices");
        }

        public void ShowDialogue(string title)
        {
            //if (title == null || title == string.Empty)
            //{
            //    ShowDoc(false);
            //    return;
            //}

            //bool canGoForward = false;
            //foreach (var dialogue in dialogueList) 
            //{
            //    if (dialogue.title == title)
            //    { 
            //        personName.text = dialogue.character;
            //        dialogueText.text = dialogue.text;

            //        if (dialogue.choices.Count > 0)
            //        {
            //            foreach (var choice in dialogue.choices) 
            //            {
            //                canGoForward = true;
            //                Button button = new()
            //                {
            //                    text = choice.text  
            //                };

            //                button.clicked += () => { ChoiceButton_clicked(button, choice); };

            //                choicePanel.Add(button);
            //            }
                      
            //        }

            //        hasChoice = canGoForward;
            //        currentDialogueNode = dialogue;
            //        break;    
            //    }
            //}
        }

        private void ChoiceButton_clicked(Button button, Choice choice)
        {
            choicePanel.Clear();
            ShowDialogue(choice.nextDialogueTitle);
        }

        
    }
}
