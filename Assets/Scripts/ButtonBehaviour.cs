using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] Sprite xSprite;
    [SerializeField] Sprite oSprite;
    [SerializeField] int posIndex;
    
    private Button button;
    private int buttonChessMark;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }
    
    //update button visual & chess array
    public void OnButtonClicked()
    {
        if (ChessBoardManager.instance.GetComponent<ChessBoardManager>().CanPlay)
        {
            if (!CheckIfOccupied())
            {
                ButtonUpdate(ChessBoardManager.instance.GetComponent<ChessBoardManager>().ChessMark);
                ChessBoardManager.instance.PlayerPlaceChess(posIndex);
            }
        }
    }

    //update button visuals & disable button interactivity 
    public void ButtonUpdate(int chessMark)
    {
        if (chessMark == 1)
        {
            button.GetComponent<Image>().sprite = oSprite;
        }
        else if (chessMark == 2)
        {
            button.GetComponent<Image>().sprite = xSprite;
        }
        
        button.interactable = false;
    }

    //check if button has been pressed
    bool CheckIfOccupied()
    {
        if (ChessBoardManager.instance.ChessbordPos[posIndex] >= 1)
            return true;
        else
            return false;
    }
    
    //reset button image sprite
    public void ResetSprite()
    {
        button.GetComponent<Image>().sprite = null;
        button.interactable = false;
    }
}
