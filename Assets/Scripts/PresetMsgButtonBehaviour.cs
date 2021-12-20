using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetMsgButtonBehaviour : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private Button button;
    private NetworkedClient networkedClient;

    private void Start()
    {
        button.GetComponent<Button>().onClick.AddListener(SendMessage);
        networkedClient = FindObjectOfType<NetworkedClient>();
    }

    public void SendMessage()
    {
        networkedClient.SendMessageToServer(ClientToServerSignifiers.sendMessage + "," + message);
    }
}
