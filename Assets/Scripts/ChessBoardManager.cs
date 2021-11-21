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

    public int[] ChessbordPos
    {
        get => chessbordPos;
    }

    [SerializeField]
    private Button[] buttonArr;

    public int chessMark;
    private int chessPlaced;
    public int playerID;
    public bool canPlay;

    // public bool CanPlay
    // {
    //     get => canPlay;
    //     set => canPlay = value;
    // }
    
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
        canPlay = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OpponentPlaceChess(3);
        }
    }

    public void PlayerPlaceChess(int index)
    {
        Debug.Log("chess placed" + canPlay);
        chessbordPos[index] = playerID;
        chessPlaced++;
        
        networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerAction + "," + index);

        canPlay = false; 

        if (isWin())
        {
            //networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.playerWin + "," + playerID);
        }
        else if (isDraw())
        {
            //networkedClient.SendMessageToHost(NetworkedClient.ClientToServerSignifiers.isDraw + "");
        }
    }

    public void OpponentPlaceChess(int index)
    {
        //Debug.Log("Opponent chess placed");

        if (chessMark == 1)
        {
            buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(chessMark + 1);
        }
        else if (chessMark == 2)
        {
            buttonArr[index].GetComponent<ButtonBehaviour>().ButtonUpdate(chessMark - 1);
        }

        chessPlaced++;
        canPlay = true;
        
        Debug.Log("Opponent chess placed:" + canPlay);
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
