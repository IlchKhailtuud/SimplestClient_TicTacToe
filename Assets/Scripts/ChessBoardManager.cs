using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    private int[] chessbordPos;

    public int[] ChessbordPos
    {
        get => chessbordPos;
    }

    [SerializeField]
    private Button[] buttonArr;

    private int chessPlaced;
    private int playerID;

    public int PlayerID
    {
        get => playerID;
        set => playerID = value;
    }

    private bool canPlay;

    public bool CanPlay
    {
        get => canPlay;
        set => canPlay = value;
    }
    
    private NetworkedClient networkedClient;

    private void OnEnable()
    {
        networkedClient = FindObjectOfType<NetworkedClient>();
    }

    void Start()
    {
        chessbordPos = new int [9];
        chessPlaced = 0;
        canPlay = true;
        playerID = 1;
    }

    void Update()
    {
    }

    public void PlayerPlaceChess(int index, int playerID)
    {
        chessbordPos[index] = playerID;
        chessPlaced++;
        
        networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerAction + "," + index + "," + playerID);
            
        canPlay = false;

        if (isWin())
        {
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerWin + "," + playerID);
        }
        else if (isDraw())
        {
            networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.isDraw + "");
        }
    }

    public void OpponentPlaceChess(int index, int opponentID)
    {
        buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(opponentID);
        
        chessPlaced++;
        canPlay = true;
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

    public bool isDraw()
    {
        if (chessPlaced >= 9)
            return true;
        else
            return false;
    }
}
