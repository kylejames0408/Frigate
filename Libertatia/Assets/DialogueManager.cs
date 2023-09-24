using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI speakerNameText;     // the public reference to the dialogue box name text (who the speaker is)
    public TextMeshProUGUI dialogueBoxText;     // the public reference to the dialogue box text (what the speaker is saying)
    public TextMeshProUGUI continueButtonText;  // the public reference to the continue button (to swap between continue and skip)

    public Animator animator;                   // the public reference to the dialogue box's animator (so it can animate on and off screen)

    public float textWriteSpeed;                // a public float so you can set how fast characters in a sentence populate the dialogue box
    
    private Queue<string> sentences;            // a Queue to hold all the sentences in the dialogue. It's like a list, but better for our use case

    private string currentSentence;             // a private string used to hold the currently displayed sentence. This is for skipping character typing

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        currentSentence = string.Empty;
    }

    // Starts the dialogue display. Brings up the box, queues up all the sentences for this dialogue, sets up the speaker name and displays the first sentence
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        speakerNameText.text = dialogue.speakerName;

        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
   
        DisplayNextSentence();
    }

    // Call this method to display the next sentence in the Queue. This is done through the continue button which calls this method
    public void DisplayNextSentence()
    {
        // if there are no sentences left to display and the last sentence has finished typing, the dialogue will end
        if (sentences.Count == 0 && dialogueBoxText.text == currentSentence)
        {
            EndDialogue();
            return;
        }

        // if the last sentence has finished typing or the first sentence hasn't been displayed yet, display the next sentence
        if(dialogueBoxText.text == currentSentence || currentSentence == string.Empty)
        {
            currentSentence = sentences.Dequeue();
            continueButtonText.text = "Skip";
            StartCoroutine(TypeSentence(currentSentence));
        }
        // else [if] the current sentence hasn't finished typing, stop typing and autofill the current sentence in the dialogue box
        else
        {
            StopAllCoroutines();
            if (sentences.Count == 0)
                continueButtonText.text = "Close";
            else
                continueButtonText.text = "Continue";
            dialogueBoxText.text = currentSentence;
        }
    }

    // A coroutine for typing the sentence so the letters can populate it over time.
    IEnumerator TypeSentence(string sentence)
    {
        // empties the dialogue box for the new sentence
        dialogueBoxText.text = "";

        // turns the sentence into a char array and loops through to type each individual letter
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueBoxText.text += letter;

            yield return new WaitForSeconds(textWriteSpeed);    // use this to write a new letter each set time step
            //yield return null;                                // use this to write a new letter each frame
        }


        if (dialogueBoxText.text == sentence)
            if (sentences.Count == 0)
                continueButtonText.text = "Close";
            else
                continueButtonText.text = "Continue";
        yield return null;

    }

    // A simple method to end the dialogue event and close the dialogue box.
    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }

}
