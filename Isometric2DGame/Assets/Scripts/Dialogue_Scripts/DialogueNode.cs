using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    public string text;
    public int nextNodeID = -1;
}

[System.Serializable]
public class DialogueNode
{
    [TextArea(3, 10)]
    public string dialogueText;

    public bool useNextButton = false; 
    public DialogueOption[] options;

    public bool isCutscene = false;
    public string cutsceneTrigger;
}
