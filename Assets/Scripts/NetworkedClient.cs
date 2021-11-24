using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkedClient : MonoBehaviour
{
    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;

    [SerializeField]GameObject gameManager;
    [SerializeField]GameObject TicTacToeManager;
    
    void Start()
    {
        Connect();
    }
    
    void Update()
    {
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessReceivedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.47", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessReceivedMsg(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.LoginResponse)
        {
            int loginResultSignifier = int.Parse(csv[1]);
            
            if (loginResultSignifier == LoginResponses.Success)
                gameManager.GetComponent<GameSystemManager>().ChangeGameStates(GameSystemManager.GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            gameManager.GetComponent<GameSystemManager>().ChangeGameStates(GameSystemManager.GameStates.PlayingTicTacToe);
            
            TicTacToeManager.GetComponent<ChessBoardManager>().playerID = int.Parse(csv[1]);

            TicTacToeManager.GetComponent<ChessBoardManager>().chessMark = int.Parse(csv[2]);
            
            if (int.Parse(csv[3]) == 1)
                TicTacToeManager.GetComponent<ChessBoardManager>().canPlay = true;
        }
        else if (signifier == ServerToClientSignifiers.OpponentTicTacToePlay)
        {
            TicTacToeManager.GetComponent<ChessBoardManager>().OpponentPlaceChess(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]));
        }
        else if (signifier == ServerToClientSignifiers.DisplayReceivedMsg)
        {
            gameManager.GetComponent<GameSystemManager>().DisplayReceivedMessage(csv[1]);
        }
        else if (signifier == ServerToClientSignifiers.spectatorJoin)
        {
            int updateSignifier = int.Parse(csv[1]);
            if (updateSignifier == 0)
            {
                gameManager.GetComponent<GameSystemManager>().ChangeGameStates(GameStates.PlayingTicTacToe);
            }
            else if (updateSignifier == 1)
            {
                int pos = int.Parse(csv[2]);
                int mark = int.Parse(csv[3]);

                TicTacToeManager.GetComponent<ChessBoardManager>().chesslist
                    .Add(new ChessBoardManager.PlayerChess(mark, pos));
            }
            else if (updateSignifier == 2)
            {
                TicTacToeManager.GetComponent<ChessBoardManager>().BulkUpdate();
            }
        }
        else if (signifier == ServerToClientSignifiers.updateSpectator)
        {
            TicTacToeManager.GetComponent<ChessBoardManager>().UpdateSpectator(int.Parse(csv[1]), int.Parse(csv[2]));
        }
        else if (signifier == ServerToClientSignifiers.announceWinner)
        {
            gameManager.GetComponent<GameSystemManager>().resultText.GetComponent<Text>().text = "Player" + csv[1] + " wins!";
            gameManager.GetComponent<GameSystemManager>().replayButton.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.announceDraw)
        {
            gameManager.GetComponent<GameSystemManager>().resultText.GetComponent<Text>().text = "It's a tie!";
            gameManager.GetComponent<GameSystemManager>().replayButton.SetActive(true);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
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
        
    }

    public static class ServerToClientSignifiers
    {
        public const int LoginResponse = 1;
        public const int GameSessionStarted = 2;
        public const int OpponentTicTacToePlay = 3;
        public const int DisplayReceivedMsg = 4;
        public const int DecideTurnOrder = 5;
        public const int spectatorJoin = 6;
        public const int updateSpectator = 7;
        public const int announceWinner = 8;
        public const int announceDraw = 9;
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
