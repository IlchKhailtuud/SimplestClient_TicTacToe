using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class NetworkedClientProcessing 
{
    #region Send and Receive Data Functions
    static public void ReceiveMessageFromServer(string msg)
    {
        Debug.Log("msg received = " + msg + ".");
        
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.LoginResponse)
        {
            int loginResultSignifier = int.Parse(csv[1]);
            
            if (loginResultSignifier == LoginResponses.Success)
                GameSystemManager.instance.ChangeGameStates(GameSystemManager.GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.GameSessionStarted)
        {
            GameSystemManager.instance.ChangeGameStates(GameSystemManager.GameStates.PlayingTicTacToe);
            
            //allocate playerID & chess mark for each player
            ChessBoardManager.instance.PlayerID = int.Parse(csv[1]);
            ChessBoardManager.instance.ChessMark = int.Parse(csv[2]);
            
            //Decide which player goes first
            if (int.Parse(csv[3]) == 1)
                ChessBoardManager.instance.CanPlay = true;
        }
        else if (signifier == ServerToClientSignifiers.OpponentTicTacToePlay)
        {
            //send other player action 
            ChessBoardManager.instance.OpponentPlaceChess(int.Parse(csv[1]), int.Parse(csv[2]), int.Parse(csv[3]));
        }
        else if (signifier == ServerToClientSignifiers.DisplayReceivedMsg)
        {
            GameSystemManager.instance.DisplayReceivedMessage(csv[1]);
        }
        else if (signifier == ServerToClientSignifiers.spectatorJoin)
        {
            int updateSignifier = int.Parse(csv[1]);
            if (updateSignifier == 0)
            {
                //if there is an available session, then goto gameplay scene
                GameSystemManager.instance.ChangeGameStates(GameStates.PlayingTicTacToe);
            }
            else if (updateSignifier == 1)
            {
                int pos = int.Parse(csv[2]);
                int mark = int.Parse(csv[3]);

                //add chess moves to local chess list 
                ChessBoardManager.instance.Chesslist
                    .Add(new ChessBoardManager.PlayerChess(pos, mark));
            }
            else if (updateSignifier == 2)
            {
                //go through the local chess list and update all buttons 
                ChessBoardManager.instance.BulkChessVisualUpdate();
            }
        }
        else if (signifier == ServerToClientSignifiers.updateSpectator)
        {
            ChessBoardManager.instance.ChessVisualUpdate(int.Parse(csv[1]), int.Parse(csv[2]));
        }
        else if (signifier == ServerToClientSignifiers.announceWinner)
        {
            //update result UI text
           GameSystemManager.instance.resultText.GetComponent<Text>().text = "Player " + csv[1] + " wins!";
            //show replay button
            GameSystemManager.instance.replayButton.SetActive(true);
            //show quit button
            GameSystemManager.instance.exitButton.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.announceWinnerForSpectator)
        {
            //update result UI text
            GameSystemManager.instance.resultText.GetComponent<Text>().text = "Player " + csv[1] + " wins!";
            Debug.Log("announce winner for spectator");
        }
        else if (signifier == ServerToClientSignifiers.announceDraw)
        {
            //update result UI text
            GameSystemManager.instance.resultText.GetComponent<Text>().text = "It's a tie!";
            //show replay button
            GameSystemManager.instance.replayButton.SetActive(true);
        }
        else if (signifier == ServerToClientSignifiers.announceDrawForSpectator)
        {
            //update result UI text
            GameSystemManager.instance.resultText.GetComponent<Text>().text = "It's a tie!";
            Debug.Log("announce draw for spectator");
        }
        else if (signifier == ServerToClientSignifiers.sendReplayChessList)
        {
            int updateSignifier = int.Parse(csv[1]);

            if (updateSignifier == 0)
            {
                //remove all elements in the chess list
                ChessBoardManager.instance.Chesslist.Clear();
                //reset all buttons on chess board
                ChessBoardManager.instance.ResetAllButtonSprite();
            }
            
            if (updateSignifier == 1)
            {
                int pos = int.Parse(csv[2]);
                int mark = int.Parse(csv[3]);
                
                //add all chess info to local chesslist
                ChessBoardManager.instance.Chesslist
                    .Add(new ChessBoardManager.PlayerChess(pos, mark));
            }
            
            if (updateSignifier == 2)
            {
                //Debug.Log("List length " + ChessBoardManager.instance.Chesslist.Count);
                
                //notify client that all chess info have been sent 
                ChessBoardManager.instance.CanReplay = true;
            }
        }
    }
    
    static public void SendMessageToServer(string msg)
    {
        networkedClient.SendMessageToServer(msg);
    }
    #endregion
    
    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Network Connection Event!");
    }
    static public void DisconnectionEvent()
    {
        Debug.Log("Network Disconnection Event!");
    }
    static public bool IsConnectedToServer()
    {
        return networkedClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkedClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkedClient.Disconnect();
    }

    #endregion
    
    #region Setup
    static NetworkedClient networkedClient;

    static public void SetNetworkedClient(NetworkedClient NetworkedClient)
    {
        networkedClient = NetworkedClient;
    }
    static public NetworkedClient GetNetworkedClient()
    {
        return networkedClient;
    }
    #endregion
}

#region Protocol Signifiers
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
    public const int sendReplayChessList = 10;
    public const int announceWinnerForSpectator = 11;
    public const int announceDrawForSpectator = 12;
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
#endregion

