using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate;
    GameObject networkedClient;
    GameObject findGameSessionButton, placeHolderGameButton;
    GameObject messageDisplay;
    GameObject TicTacToe;

    void Start()
    {
        GameObject[] allObjs = FindObjectsOfType<GameObject>();

        foreach (var go in allObjs)
        {
            if (go.name == "inputFieldUserName")
                inputFieldUserName = go;
            else if (go.name == "inputFieldPassword")
                inputFieldPassword = go;
            else if (go.name == "buttonSubmit")
                buttonSubmit = go;
            else if (go.name == "toggleCreate")
                toggleCreate = go;
            else if (go.name == "toggleLogin")
                toggleLogin = go;
            else if (go.name == "networkedClient")
                networkedClient = go;
            else if (go.name == "FindGameSessionButton")
                findGameSessionButton = go;
            else if (go.name == "PlaceHolderGameButton")
                placeHolderGameButton = go;
            else if (go.name == "TicTacToe")
                TicTacToe = go;
            else if (go.name == "MessageDisplay")
                messageDisplay = go;
        }
        
        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed); 
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        
        findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed); 
        placeHolderGameButton.GetComponent<Button>().onClick.AddListener(PlaceHolderGameButtonPressed); 
        
        ChangeGameStates(GameStates.login);
    }
    
    void Update()
    {
        
    }

    private void SubmitButtonPressed()
    {
        string n = inputFieldUserName.GetComponent<InputField>().text;
        string p = inputFieldPassword.GetComponent<InputField>().text;

        if (toggleLogin.GetComponent<Toggle>().isOn)
            networkedClient.GetComponent<NetworkedClient>()
                .SendMessageToHost(ClientToServerSignifiers.Login + "," + n + "," + p);
        else
            networkedClient.GetComponent<NetworkedClient>()
                .SendMessageToHost(ClientToServerSignifiers.CreateAccount + "," + n + "," + p);
    }
    
    private void ToggleCreateValueChanged(bool newValue)
    {
        toggleLogin.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    
    private void ToggleLoginValueChanged(bool newValue)
    {
        toggleCreate.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }
    
    private void FindGameSessionButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.AddToGameSessionQueue + "");
        ChangeGameStates(GameStates.WaitingForMatch);
    }
    
    private void PlaceHolderGameButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToePlay + "");
    }

    public void ChangeGameStates(int newState)
    {
        inputFieldUserName.SetActive(false);
        inputFieldPassword.SetActive(false);
        buttonSubmit.SetActive(false);
        toggleLogin.SetActive(false);
        toggleCreate.SetActive(false);
        findGameSessionButton.SetActive(false);
        placeHolderGameButton.SetActive(false);
        

        if (newState == GameStates.login)
        {
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            buttonSubmit.SetActive(true);
            toggleLogin.SetActive(true);
            toggleCreate.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            findGameSessionButton.SetActive(true);
        }
        else if (newState == GameStates.WaitingForMatch)
        {
            
        }
        else if (newState == GameStates.PlayingTicTacToe)
        {
            placeHolderGameButton.SetActive(true);
        }
       
    }

    public void DisplayReceivedMessage(string message)
    {
        messageDisplay.GetComponent<Text>().text = message;
    }

    public static class ClientToServerSignifiers
    {
        public const int Login = 1;
        public const int CreateAccount = 2;
        public const int AddToGameSessionQueue = 3;
        public const int TicTacToePlay = 4;
        public const int playerAction = 3;
        public const int playerWin = 4;
        public const int isDraw = 5;
        public const int sendMessage = 6;
    }

    public static class ServerToClientSignifiers
    {
        public const int LoginResponse = 1;
        public const int displayMessage = 2;
        public const int GameSessionStarted = 3;
        public const int OpponentTicTacToePlay = 4;
        public const int DisplayReceivedMsg = 5;
    }
 
    public static class LoginResponses
    {
        public const int Success = 1;
        public const int FailureNameInUse = 2;
        public const int FailureNameNotFound = 3;
        public const int FailureIncorrectPassword = 4; 
    }

    public static class GameStates
    {
        public const int login = 1;
        public const int MainMenu = 2;
        public const int WaitingForMatch = 3;
        public const int PlayingTicTacToe = 4;
    }
}
