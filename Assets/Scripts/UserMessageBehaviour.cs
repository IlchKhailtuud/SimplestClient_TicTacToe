using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMessageBehaviour : MonoBehaviour
{
   [SerializeField ]private InputField userInputField;
   [SerializeField ]private Button sendButton;
   private NetworkedClient networkedClient;

   private void Start()
   {
      networkedClient = FindObjectOfType<NetworkedClient>();
      sendButton.onClick.AddListener(SendUserMessage);
   }

   public void SendUserMessage()
   {
      if (userInputField.text != null)
      {
         networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.sendMessage + "," + userInputField.text);
         Debug.Log("userInputField.text)");
      }
   }
}
