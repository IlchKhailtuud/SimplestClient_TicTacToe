using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    public static ChessBoardManager instance;

    private int[] chessbordPos;
    public List<PlayerChess> chesslist;

    public int[] ChessbordPos
    {
        get => chessbordPos;
    }

    [SerializeField] private Button[] buttonArr;

    public int chessMark;
    private int chessPlaced;
    public int playerID;
    public bool canPlay;

    private NetworkedClient networkedClient;

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
        chessbordPos = new int [9];
        chessPlaced = 0;
        chesslist = new List<PlayerChess>();
    }
    
    public void PlayerPlaceChess(int index)
    {
        chessbordPos[index] = playerID;
        chessPlaced++;

        networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerAction + "," + index + "," + chessMark);

        canPlay = false;

        if (isWin())
        {
            Debug.Log("Winnnnnnnn!");
            
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerWin + "," + playerID);
        }
        else if (chessPlaced >= 9)
        {
            Debug.Log("Drawwwwwwww!");
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.isDraw + "");
        }
    }

    public void OpponentPlaceChess(int index, int mark, int playerid)
    {
        buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(mark);
        chessbordPos[index] = playerid;
        chessPlaced++;
        canPlay = true;
    }
    
    public void UpdateSpectator(int index, int mark)
    {
        buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(mark);
    }
    
    public void BulkUpdate()
    {
        for (int i = 0; i < chesslist.Count; i++)
        {
            UpdateSpectator(chesslist[i].chessPos, chesslist[i].chessMark);
        }
    }

    public bool isWin()
    {
        if ((chessbordPos[0] == playerID && chessbordPos[1] == playerID && chessbordPos[2] == playerID)
            || (chessbordPos[3] == playerID && chessbordPos[4] == playerID && chessbordPos[5] == playerID)
            || (chessbordPos[6] == playerID && chessbordPos[7] == playerID && chessbordPos[8] == playerID)
            || (chessbordPos[0] == playerID && chessbordPos[3] == playerID && chessbordPos[6] == playerID)
            || (chessbordPos[1] == playerID && chessbordPos[4] == playerID && chessbordPos[7] == playerID)
            || (chessbordPos[2] == playerID && chessbordPos[5] == playerID && chessbordPos[8] == playerID)
            || (chessbordPos[0] == playerID && chessbordPos[4] == playerID && chessbordPos[8] == playerID)
            || (chessbordPos[2] == playerID && chessbordPos[4] == playerID && chessbordPos[6] == playerID))
        {
            canPlay = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public class PlayerChess
    {
        public int chessMark;
        public int chessPos;

        public PlayerChess(int chessMark, int chessPos)
        {
            this.chessMark = chessMark;
            this.chessPos = chessPos;
        }
    }
}

