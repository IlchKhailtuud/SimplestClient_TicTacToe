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
    
    void Start()
    {
        if (NetworkedClientProcessing.GetNetworkedClient() == null)
        {
            DontDestroyOnLoad(this.gameObject);
            NetworkedClientProcessing.SetNetworkedClient(this);
            Connect();
        }
        else
        {
            Debug.Log("Singleton-ish architecture violation detected, investigate where NetworkedClient.cs Start() is being called.  Are you creating a second instance of the NetworkedClient game object or has the NetworkedClient.cs been attached to more than one game object?");
            Destroy(this.gameObject);
        }
    }
    
    void Update()
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
                    NetworkedClientProcessing.ReceiveMessageFromServer(msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    public void Connect()
    {
        if (!isConnected)
        {
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
            else
                Debug.Log("Network client init failed!");
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public void SendMessageToServer(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }
    
    public bool IsConnected()
    {
        return isConnected;
    }
    
    // private void UpdateNetworkConnection()
    // {
    //     if (isConnected)
    //     {
    //         int recHostID;
    //         int recConnectionID;
    //         int recChannelID;
    //         byte[] recBuffer = new byte[1024];
    //         int bufferSize = 1024;
    //         int dataSize;
    //         NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);
    //
    //         switch (recNetworkEvent)
    //         {
    //             case NetworkEventType.ConnectEvent:
    //                 Debug.Log("connected.  " + recConnectionID);
    //                 ourClientID = recConnectionID;
    //                 break;
    //             case NetworkEventType.DataEvent:
    //                 string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
    //                 NetworkClientProcessing.ReceiveMessageFromServer(msg);
    //                 break;
    //             case NetworkEventType.DisconnectEvent:
    //                 isConnected = false;
    //                 Debug.Log("disconnected.  " + recConnectionID);
    //                 break;
    //         }
    //     }
    // }
    // private void ProcessReceivedMsg(string msg, int id)
    // {
    //     Debug.Log("msg received = " + msg + ".  connection id = " + id);
    //
    //     // string[] csv = msg.Split(',');
    //     // int signifier = int.Parse(csv[0]);
    //     //
    //     // if (signifier == ServerToClientSignifiers.LoginResponse)
    //     // {
    //     //     int loginResultSignifier = int.Parse(csv[1]);
    //     //     
    //     //     if (loginResultSignifier == LoginResponses.Success)
    //     //         gameManager.GetComponent<GameSystemManager>().ChangeGameStates(GameSystemManager.GameStates.MainMenu);
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.GameSessionStarted)
    //     // {
    //     //     gameManager.GetComponent<GameSystemManager>().ChangeGameStates(GameSystemManager.GameStates.PlayingTicTacToe);
    //     //     
    //     //     //allocate playerID & chess mark for each player
    //     //     TicTacToeManager.GetComponent<ChessBoardManager>().PlayerID = int.Parse(csv[1]);
    //     //     TicTacToeManager.GetComponent<ChessBoardManager>().ChessMark = int.Parse(csv[2]);
    //     //     
    //     //     //Decide which player goes first
    //     //     if (int.Parse(csv[3]) == 1)
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().CanPlay = true;
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.OpponentTicTacToePlay)
    //     // {
    //     //     //send other player action 
    //     //     TicTacToeManager.GetComponent<ChessBoardManager>().OpponentPlaceChess(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]));
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.DisplayReceivedMsg)
    //     // {
    //     //     gameManager.GetComponent<GameSystemManager>().DisplayReceivedMessage(csv[1]);
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.spectatorJoin)
    //     // {
    //     //     int updateSignifier = int.Parse(csv[1]);
    //     //     if (updateSignifier == 0)
    //     //     {
    //     //         //if there is an available session, then goto gameplay scene
    //     //         gameManager.GetComponent<GameSystemManager>().ChangeGameStates(GameStates.PlayingTicTacToe);
    //     //     }
    //     //     else if (updateSignifier == 1)
    //     //     {
    //     //         int pos = int.Parse(csv[2]);
    //     //         int mark = int.Parse(csv[3]);
    //     //
    //     //         //add chess moves to local chess list 
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().chesslist
    //     //             .Add(new ChessBoardManager.PlayerChess(pos, mark));
    //     //     }
    //     //     else if (updateSignifier == 2)
    //     //     {
    //     //         //go through the local chess list and update all buttons 
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().BulkChessVisualUpdate();
    //     //     }
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.updateSpectator)
    //     // {
    //     //     TicTacToeManager.GetComponent<ChessBoardManager>().ChessVisualUpdate(int.Parse(csv[1]), int.Parse(csv[2]));
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.announceWinner)
    //     // {
    //     //     //update result UI text
    //     //     gameManager.GetComponent<GameSystemManager>().resultText.GetComponent<Text>().text = "Player " + csv[1] + " wins!";
    //     //     //show replay button
    //     //     gameManager.GetComponent<GameSystemManager>().replayButton.SetActive(true);
    //     //     //show quit button
    //     //     gameManager.GetComponent<GameSystemManager>().exitButton.SetActive(true);
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.announceWinnerForSpectator)
    //     // {
    //     //     //update result UI text
    //     //     gameManager.GetComponent<GameSystemManager>().resultText.GetComponent<Text>().text = "Player " + csv[1] + " wins!";
    //     //     Debug.Log("announce winner for spectator");
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.announceDraw)
    //     // {
    //     //     //update result UI text
    //     //     gameManager.GetComponent<GameSystemManager>().resultText.GetComponent<Text>().text = "It's a tie!";
    //     //     //show replay button
    //     //     gameManager.GetComponent<GameSystemManager>().replayButton.SetActive(true);
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.announceDrawForSpectator)
    //     // {
    //     //     //update result UI text
    //     //     gameManager.GetComponent<GameSystemManager>().resultText.GetComponent<Text>().text = "It's a tie!";
    //     //     Debug.Log("announce draw for spectator");
    //     // }
    //     // else if (signifier == ServerToClientSignifiers.sendReplayChessList)
    //     // {
    //     //     int updateSignifier = int.Parse(csv[1]);
    //     //
    //     //     if (updateSignifier == 0)
    //     //     {
    //     //         //remove all elements in the chess list
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().chesslist.Clear();
    //     //         //reset all buttons on chess board
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().ResetAllButtonSprite();
    //     //     }
    //     //     
    //     //     if (updateSignifier == 1)
    //     //     {
    //     //         int pos = int.Parse(csv[2]);
    //     //         int mark = int.Parse(csv[3]);
    //     //         
    //     //         //add all chess info to local chesslist
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().chesslist
    //     //             .Add(new ChessBoardManager.PlayerChess(pos, mark));
    //     //     }
    //     //     
    //     //     if (updateSignifier == 2)
    //     //     {
    //     //         Debug.Log("List length " + TicTacToeManager.GetComponent<ChessBoardManager>().chesslist.Count);
    //     //         
    //     //         //notify client that all chess info have been sent 
    //     //         TicTacToeManager.GetComponent<ChessBoardManager>().CanReplay = true;
    //     //     }
    //     // }
    // }
    
    // public static class ClientToServerSignifiers
    // {
    //     public const int Login = 1;
    //     public const int CreateAccount = 2;
    //     public const int AddToGameSessionQueue = 3;
    //     public const int TicTacToePlay = 4;
    //     public const int playerAction = 5;
    //     public const int playerWin = 6;
    //     public const int isDraw = 7;
    //     public const int sendMessage = 8;
    //     public const int watchGame = 9;
    //     public const int requestReplay = 10;
    //     public const int startNewSession = 11;
    // }
    //
    // public static class ServerToClientSignifiers
    // {
    //     public const int LoginResponse = 1;
    //     public const int GameSessionStarted = 2;
    //     public const int OpponentTicTacToePlay = 3;
    //     public const int DisplayReceivedMsg = 4;
    //     public const int DecideTurnOrder = 5;
    //     public const int spectatorJoin = 6;
    //     public const int updateSpectator = 7;
    //     public const int announceWinner = 8;
    //     public const int announceDraw = 9;
    //     public const int sendReplayChessList = 10;
    //     public const int announceWinnerForSpectator = 11;
    //     public const int announceDrawForSpectator = 12;
    // } 
    //
    // public static class LoginResponses
    // {
    //     public const int Success = 1;
    //     public const int FailureNameInUse = 2;
    //     public const int FailureNameNotFound = 3;
    //     public const int FailureIncorrectPassword = 4; 
    // }
    //
    // public static class GameStates
    // {
    //     public const int login = 1;
    //     public const int MainMenu = 2;
    //     public const int WaitingForMatch = 3;
    //     public const int PlayingTicTacToe = 4;
    // }
}
