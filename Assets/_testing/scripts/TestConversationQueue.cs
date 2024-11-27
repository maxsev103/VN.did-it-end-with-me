using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConversationQueue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        List<string> lines = new List<string>()
        {
            "Line 1 from init convo",
            "Line 2 from init convo",
            "Line 3 from init convo"
        };

        yield return DialogueSystem.instance.Say(lines);

        DialogueSystem.instance.Hide();
    }

    private void Update()
    {
        List<string> lines = new List<string>();
        Conversation conversation = null;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            lines = new List<string>()
            {
                "This is the start of an enqueued convo",
                "we can keep it going or whatever"
            };
            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.Enqueue(conversation);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            lines = new List<string>()
            {
                "This is an important convo that takes priority!!!",
                "October 29th is a very special day."
            };
            conversation = new Conversation(lines);
            DialogueSystem.instance.conversationManager.EnqueuePriority(conversation);
        }
    }
}
