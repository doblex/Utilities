using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Linq;

public class DialogueEditorWindow : EditorWindow
{
    private List<DialogueData> dialogues = new();
    private Vector2 scroll;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow()
    {
        GetWindow<DialogueEditorWindow>("Dialogue Editor");
    }

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("\ud83d\udcc2 Carica XML"))
        {
            LoadXml();
        }
        if (GUILayout.Button("\ud83d\udcc5 Salva XML"))
        {
            if (ValidateIDs())
                SaveXml();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("\u2795 Aggiungi Dialogo"))
        {
            dialogues.Add(new DialogueData());
        }

        for (int i = 0; i < dialogues.Count; i++)
        {
            DrawDialogue(dialogues[i], i);
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawDialogue(DialogueData dialogue, int index)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Dialogo #" + (index + 1), EditorStyles.boldLabel);

        dialogue.id = EditorGUILayout.TextField("ID", dialogue.id);
        dialogue.character = EditorGUILayout.TextField("Personaggio", dialogue.character);

        EditorGUILayout.BeginHorizontal();
        dialogue.text = EditorGUILayout.TextField("Testo", dialogue.text);
        if (GUILayout.Button("Highlight", GUILayout.Width(80)))
        {
            dialogue.text += "<highlight color=\"yellow\">TESTO</highlight>";
        }
        EditorGUILayout.EndHorizontal();

        dialogue.isEnd = EditorGUILayout.Toggle("Fine Dialogo (isEnd)", dialogue.isEnd);

        if (!dialogue.isEnd)
        {
            if (dialogue.choices.Count > 0)
            {
                EditorGUILayout.LabelField("Scelte:");
                for (int j = 0; j < dialogue.choices.Count; j++)
                {
                    var choice = dialogue.choices[j];
                    EditorGUILayout.BeginHorizontal();
                    choice.text = EditorGUILayout.TextField("Testo", choice.text);
                    choice.nextId = EditorGUILayout.TextField("Next ID", choice.nextId);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        dialogue.choices.RemoveAt(j);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("\u2795 Aggiungi Scelta"))
                {
                    dialogue.choices.Add(new ChoiceData());
                }
            }
            else
            {
                dialogue.nextId = EditorGUILayout.TextField("Next ID", dialogue.nextId);

                if (GUILayout.Button("\u2795 Aggiungi Scelta"))
                {
                    dialogue.choices.Add(new ChoiceData());
                    dialogue.nextId = "";
                }
            }
        }
        else
        {
            dialogue.nextId = EditorGUILayout.TextField("Next ID", dialogue.nextId);
        }

        if (GUILayout.Button("\u274c Rimuovi Dialogo"))
        {
            dialogues.RemoveAt(index);
            return;
        }

        EditorGUILayout.EndVertical();
    }

    bool ValidateIDs()
    {
        HashSet<string> allIds = new(dialogues.Select(d => d.id));
        bool isValid = true;

        foreach (var dialogue in dialogues)
        {
            if (!dialogue.isEnd && dialogue.choices.Count > 0)
            {
                foreach (var choice in dialogue.choices)
                {
                    if (!allIds.Contains(choice.nextId))
                    {
                        Debug.LogError($"\u274c ID mancante: scelta '{choice.text}' nel dialogo '{dialogue.id}' punta a '{choice.nextId}', che non esiste.");
                        isValid = false;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(dialogue.nextId))
            {
                if (!allIds.Contains(dialogue.nextId))
                {
                    Debug.LogError($"\u274c ID mancante: il dialogo '{dialogue.id}' punta a '{dialogue.nextId}', che non esiste.");
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    void SaveXml()
    {
        string path = EditorUtility.SaveFilePanel("Salva Dialoghi XML", "Assets", "dialogues", "xml");
        if (string.IsNullOrEmpty(path)) return;

        XElement root = new XElement("dialogues");

        foreach (var dialogue in dialogues)
        {
            XElement dialogueElem = new XElement("dialogue",
                new XAttribute("id", dialogue.id),
                new XAttribute("isEnd", dialogue.isEnd.ToString().ToLower())
            );

            XElement line = new XElement("line",
                new XElement("character", dialogue.character),
                CreateTextElement(dialogue.text)
            );

            if (!dialogue.isEnd)
            {
                if (dialogue.choices.Count > 0)
                {
                    XElement choices = new XElement("choices");
                    foreach (var choice in dialogue.choices)
                    {
                        choices.Add(new XElement("choice",
                            new XAttribute("text", choice.text),
                            new XAttribute("next", choice.nextId)
                        ));
                    }
                    line.Add(choices);
                }
                else if (!string.IsNullOrEmpty(dialogue.nextId))
                {
                    line.Add(new XElement("next", new XAttribute("next", dialogue.nextId)));
                }
            }
            else if (!string.IsNullOrEmpty(dialogue.nextId))
            {
                line.Add(new XElement("next", new XAttribute("next", dialogue.nextId)));
            }

            dialogueElem.Add(line);
            root.Add(dialogueElem);
        }

        File.WriteAllText(path, root.ToString());
        AssetDatabase.Refresh();
        Debug.Log("\u2705 XML salvato in: " + path);
    }

    void LoadXml()
    {
        string path = EditorUtility.OpenFilePanel("Carica XML", "Assets", "xml");
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            XDocument doc = XDocument.Load(path);
            var dialogueElems = doc.Root.Elements("dialogue");
            dialogues.Clear();

            foreach (var dElem in dialogueElems)
            {
                DialogueData dialogue = new DialogueData();
                dialogue.id = dElem.Attribute("id")?.Value ?? "";
                dialogue.isEnd = dElem.Attribute("isEnd")?.Value == "true";

                var lineElem = dElem.Element("line");
                dialogue.character = lineElem.Element("character")?.Value ?? "";

                var textElem = lineElem.Element("text");
                dialogue.text = textElem?.Value ?? "";
                if (textElem?.Nodes().OfType<XCData>().Any() == true)
                {
                    dialogue.text = textElem.Value;
                }

                var choicesElem = lineElem.Element("choices");
                if (choicesElem != null)
                {
                    foreach (var choiceElem in choicesElem.Elements("choice"))
                    {
                        dialogue.choices.Add(new ChoiceData
                        {
                            text = choiceElem.Attribute("text")?.Value ?? "",
                            nextId = choiceElem.Attribute("next")?.Value ?? ""
                        });
                    }
                }

                var nextElem = lineElem.Element("next");
                if (nextElem != null)
                {
                    dialogue.nextId = nextElem.Attribute("next")?.Value ?? "";
                }

                dialogues.Add(dialogue);
            }

            Debug.Log("\u2705 Dialoghi caricati con successo da: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("\u274c Errore nel parsing del file XML: " + e.Message);
        }
    }

    XElement CreateTextElement(string text)
    {
        XElement textElem = new XElement("text");

        if (text.Contains("<highlight"))
        {
            textElem.Add(new XCData(text));
        }
        else
        {
            textElem.Value = text;
        }

        return textElem;
    }
}

[System.Serializable]
public class DialogueData
{
    public string id = "";
    public string character = "";
    public string text = "";
    public bool isEnd = false;
    public string nextId = "";
    public List<ChoiceData> choices = new();
}

[System.Serializable]
public class ChoiceData
{
    public string text = "";
    public string nextId = "";
}
