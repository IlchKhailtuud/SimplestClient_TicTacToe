using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMessageBehaviour : MonoBehaviour
{
   private InputField userInputField;
   private Button sendButton;
   private NetworkedClient networkedClient;

   private void Start()
   {
      userInputField = GetComponentInChildren<InputField>();
      sendButton = GetComponent<Button>();
      networkedClient = FindObjectOfType<NetworkedClient>();

      sendButton.onClick.AddListener(SendUserMessage);
   }

   public void SendUserMessage()
   {
      if (userInputField.text != null)
      {
         networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.sendMessage + "," + userInputField.text);
      }
   }
}
