using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public int id;
    public string dialogue;
    public string trigger;
}

public class DialogueParser : MonoBehaviour
{
    public static List<DialogueLine> ParseCSV(string csvText)
    {
        List<DialogueLine> dialogueLines = new List<DialogueLine>();
        string[] lines = csvText.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            if (values.Length >= 3)
            {
                DialogueLine line = new DialogueLine();
                line.id = int.Parse(values[0].Trim());
                line.dialogue = values[1].Trim();
                line.trigger = values[2].Trim();

                dialogueLines.Add(line);
            }
        }

        return dialogueLines;
    }

    public static List<DialogueLine> GetDialogueSequence(List<DialogueLine> allLines, int startId)
    {
        List<DialogueLine> sequence = new List<DialogueLine>();
        bool collecting = false;

        foreach (DialogueLine line in allLines)
        {
            if (line.id == startId && line.trigger == "start")
            {
                collecting = true;
                sequence.Add(line);
            }
            else if (collecting && line.trigger == "entry")
            {
                sequence.Add(line);
            }
            else if (collecting && line.trigger == "end")
            {
                sequence.Add(line);
                break;
            }
        }

        return sequence;
    }
}
