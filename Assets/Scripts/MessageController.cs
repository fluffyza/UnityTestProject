using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public InputField userMessage;
    public Button sendButton;
    public Message messagePrefab;
    Transform content;


    // Start is called before the first frame update
    void Start()
    {
		Button btn = sendButton.GetComponent<Button>();
		btn.onClick.AddListener(SendMessage);
        SetContentObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendMessage()
    {
        string msg = userMessage.text;
        //Send message to UI
        //Contact server to retrieve the audio files/ response
        // push audio arr through to the VisualScript
        // push return message to UI
        Message newMessageSpawned = Instantiate(messagePrefab, content);
        newMessageSpawned.messageText.text = msg;
    }

    public void SetContentObject()
    {
        content = GameObject.FindGameObjectWithTag("Content").transform;
    }
}
