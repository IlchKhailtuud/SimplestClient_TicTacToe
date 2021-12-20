using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetMsgButtonBehaviour : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private Button button;

    private void Start()
    {
        button.GetComponent<Button>().onClick.AddListener(SendMessage);
    }

    public void SendMessage()
    {
        NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.sendMessage + "," + message);
    }
}
