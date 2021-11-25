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
    public bool canReplay;
    private float passedTime;
    private float targetTime;
    private int listIndex;
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
        passedTime = 0.0f;
        targetTime = 0.5f;
        listIndex = 0;
    }

    private void Update()
    {
        if (canReplay && listIndex < chesslist.Count)
        {
            if (passedTime >= targetTime)
            {
                ChessVisualUpdate(chesslist[listIndex].chessPos, chesslist[listIndex].chessMark);
                
                ++listIndex;
                passedTime = 0f;
            }
            passedTime += Time.deltaTime;
        }
    }

    public void PlayerPlaceChess(int index)
    {
        chessbordPos[index] = playerID;
        chessPlaced++;

        networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerAction + "," + index + "," + chessMark);
        
        canPlay = false;

        if (isWin())
        {
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerWin + "," + playerID);
        }
        else if (chessPlaced >= 9)
        {
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
    
    public void ChessVisualUpdate(int index, int mark)
    {
        buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(mark);
    }
    
    public void BulkUpdate()
    {
        for (int i = 0; i < chesslist.Count; i++)
        {
            ChessVisualUpdate(chesslist[i].chessPos, chesslist[i].chessMark);
        }
    }
    
    public void ResetAllButtons()
    {
        foreach (Button button in buttonArr)
        {
            button.GetComponent<ButtonBehaviour>().ResetSprite();
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

        public PlayerChess(int chessPos, int chessMark)
        {
            this.chessMark = chessMark;
            this.chessPos = chessPos;
        }
    }
}

