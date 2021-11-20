using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetMsgButtonBehaviour : MonoBehaviour
{
    [SerializeField] private string message;
    private Button button;
    private NetworkedClient networkedClient;

    private void Start()
    {
        button = GetComponent<Button>();
        networkedClient = FindObjectOfType<NetworkedClient>();
        
    }

    public void SendMessage()
    {
        networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.sendMessage + "," + message);
    }
}
