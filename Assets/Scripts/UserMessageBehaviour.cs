using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMessageBehaviour : MonoBehaviour
{
   [SerializeField ]private InputField userInputField;
   [SerializeField ]private Button sendButton;

   private void Start()
   {
      sendButton.onClick.AddListener(SendUserMessage);
   }

   public void SendUserMessage()
   {
      if (userInputField.text != null)
      {
         NetworkedClientProcessing.SendMessageToServer(ClientToServerSignifiers.sendMessage + "," + userInputField.text);
         Debug.Log("userInputField.text)");
      }
   }
}
