using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject inputFieldUserName, inputFieldPassword, buttonSubmit, toggleLogin, toggleCreate, spectatorJoin;
    GameObject networkedClient;
    GameObject findGameSessionButton;
    public GameObject replayButton,  resultText, exitButton;
    public GameObject messageDisplay;
    public GameObject TicTacToe;

    void Start()
    {
        GameObject[] allObjs = FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjs)
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
            else if (go.name == "SpectatorJoin")
                spectatorJoin = go;
            else if (go.name == "ReplayButton")
                replayButton = go;
            else if (go.name == "ResultText")
                resultText = go; 
            else if (go.name == "ExitButton")
                exitButton = go;
        }
        
        buttonSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed); 
        toggleCreate.GetComponent<Toggle>().onValueChanged.AddListener(ToggleCreateValueChanged);
        toggleLogin.GetComponent<Toggle>().onValueChanged.AddListener(ToggleLoginValueChanged);
        findGameSessionButton.GetComponent<Button>().onClick.AddListener(FindGameSessionButtonPressed);
        spectatorJoin.GetComponent<Button>().onClick.AddListener(SpectatorJoinButtonPressed);
        replayButton.GetComponent<Button>().onClick.AddListener(ReplayButtonPressed);
        exitButton.GetComponent<Button>().onClick.AddListener(ExitButtonPressed);

        ChangeGameStates(GameStates.login);
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

    private void SpectatorJoinButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.watchGame + "");
    }

    public void ReplayButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.requestReplay + "");
    }

    public void ExitButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.startNewSession + "");
        ChangeGameStates(GameStates.MainMenu);
        //Application.Quit();
    }

    public void ChangeGameStates(int newState)
    {
        inputFieldUserName.SetActive(false);
        inputFieldPassword.SetActive(false);
        buttonSubmit.SetActive(false);
        toggleLogin.SetActive(false);
        toggleCreate.SetActive(false);
        findGameSessionButton.SetActive(false);
        TicTacToe.SetActive(false);
        messageDisplay.SetActive(false);
        spectatorJoin.SetActive(false);
        replayButton.SetActive(false);
        exitButton.SetActive(false);

        if (newState == GameStates.login)
        {
            inputFieldUserName.SetActive(true);
            inputFieldPassword.SetActive(true);
            buttonSubmit.SetActive(true);
            toggleLogin.SetActive(true);
            toggleCreate.SetActive(true);
            spectatorJoin.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            findGameSessionButton.SetActive(true);
        }
        else if (newState == GameStates.PlayingTicTacToe)
        {
            TicTacToe.SetActive(true);
            messageDisplay.SetActive(true);
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
        public const int playerAction = 5;
        public const int playerWin = 6;
        public const int isDraw = 7;
        public const int sendMessage = 8;
        public const int watchGame = 9;
        public const int requestReplay = 10;
        public const int startNewSession = 11;
    }
    
    public static class GameStates
    {
        public const int login = 1;
        public const int MainMenu = 2;
        public const int WaitingForMatch = 3;
        public const int PlayingTicTacToe = 4;
    }
}
