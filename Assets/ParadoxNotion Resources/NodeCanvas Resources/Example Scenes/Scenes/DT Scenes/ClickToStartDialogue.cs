using UnityEngine;
using System.Collections;
using NodeCanvas.DialogueTrees;

public class ClickToStartDialogue : MonoBehaviour
{

    public DialogueTreeController dialogueController;

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			if (!dialogueController.isRunning)
			{
				dialogueController.StartDialogue();
			}
		}
	}
}