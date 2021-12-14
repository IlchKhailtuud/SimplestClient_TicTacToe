using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    public static ChessBoardManager instance;
    
    [SerializeField] private Button[] buttonArr;
    
    //array for chess index
    private int[] chessbordArr;
    
    //list for containing both player's chess info
    public List<PlayerChess> chesslist;
    
    private int playerID;
    private int chessMark;
    private int chessPlaced;
    private bool canPlay;
    private bool canReplay;
    private float passedTime;
    private float targetTime;
    private int listIndex;
    private NetworkedClient networkedClient;
    
    public int[] ChessbordPos
    {
        get => chessbordArr;
    }
    
    public int PlayerID
    {
        get => playerID;
        set => playerID = value;
    }

    public int ChessMark
    {
        get => chessMark;
        set => chessMark = value;
    }

    public int ChessPlaced
    {
        get => chessPlaced;
        set => chessPlaced = value;
    }

    public bool CanPlay
    {
        get => canPlay;
        set => canPlay = value;
    }

    public bool CanReplay
    {
        get => canReplay;
        set => canReplay = value;
    }
    
    //singleton for Chessboard manager
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        networkedClient = FindObjectOfType<NetworkedClient>();
    }

    void Start()
    {
        chessbordArr = new int [9];
        chessPlaced = 0;
        chesslist = new List<PlayerChess>();
        passedTime = 0.0f;
        targetTime = 0.5f;
        listIndex = 0;
    }

    private void Update()
    {
        if (canReplay)
        {
            Replay();
        }
    }

    //send player's action to server & check win condition
    public void PlayerPlaceChess(int index)
    {
        chessbordArr[index] = playerID;
        chessPlaced++;
        canPlay = false;

        //send playerAction signifier to server
        networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerAction + "," + index + "," + chessMark);
        
        if (isWin())
        {
            //send playerWin signifier to server
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerWin + "," + playerID);
        }
        else if (chessPlaced >= 9)
        {
            //if chessplaced is greater than 9 && the player doesn't win then send isDraw sigifier to server
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.isDraw + "");
        }
    }

    //update button & chessboard array based on index & mark
    //set current player as the next player to place chess
    //increment chess placed
    public void OpponentPlaceChess(int index, int mark, int playerid)
    {
        buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(mark);
        chessbordArr[index] = playerid;
        chessPlaced++;
        canPlay = true;
    }
    
    //Update button visual for the observer
    public void ChessVisualUpdate(int index, int mark)
    {
        buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(mark);
    }
    
    //go through the chess list to 
    public void BulkChessVisualUpdate()
    {
        for (int i = 0; i < chesslist.Count; i++)
        {
            ChessVisualUpdate(chesslist[i].chessPos, chesslist[i].chessMark);
        }
    }
    
    //reset button to original state
    public void ResetAllButtons()
    {
        foreach (Button button in buttonArr)
        {
            button.GetComponent<ButtonBehaviour>().ResetSprite();
        }
    }

    //Replay both player moves from the start 
    private void Replay()
    {
        if (listIndex < chesslist.Count && passedTime >= targetTime)
        {
            ChessVisualUpdate(chesslist[listIndex].chessPos, chesslist[listIndex].chessMark);
                
            listIndex++;
            passedTime = 0f;
        }
        passedTime += Time.deltaTime;
    }
    
    //win condition check
    public bool isWin()
    {
        if ((chessbordArr[0] == playerID && chessbordArr[1] == playerID && chessbordArr[2] == playerID)
            || (chessbordArr[3] == playerID && chessbordArr[4] == playerID && chessbordArr[5] == playerID)
            || (chessbordArr[6] == playerID && chessbordArr[7] == playerID && chessbordArr[8] == playerID)
            || (chessbordArr[0] == playerID && chessbordArr[3] == playerID && chessbordArr[6] == playerID)
            || (chessbordArr[1] == playerID && chessbordArr[4] == playerID && chessbordArr[7] == playerID)
            || (chessbordArr[2] == playerID && chessbordArr[5] == playerID && chessbordArr[8] == playerID)
            || (chessbordArr[0] == playerID && chessbordArr[4] == playerID && chessbordArr[8] == playerID)
            || (chessbordArr[2] == playerID && chessbordArr[4] == playerID && chessbordArr[6] == playerID))
        {
            canPlay = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    //class for containing chess info 
    public class PlayerChess
    {
        public int chessMark;
        public int chessPos;

        public PlayerChess(int chessPos, int chessMark)
        {
            this.chessMark = chessMark;
            this.chessPos = chessPos;
        }
    }
}

