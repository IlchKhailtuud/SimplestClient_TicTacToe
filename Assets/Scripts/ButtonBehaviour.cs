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

    public void OnButtonClicked()
    {
        if (ChessBoardManager.instance.GetComponent<ChessBoardManager>().canPlay)
        {
            if (!CheckIfOccupied())
            {
                ButtonUpdate(ChessBoardManager.instance.GetComponent<ChessBoardManager>().chessMark);
                ChessBoardManager.instance.PlayerPlaceChess(posIndex);
            }
        }
    }

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

    bool CheckIfOccupied()
    {
        if (ChessBoardManager.instance.ChessbordPos[posIndex] >= 1)
            return true;
        else
            return false;
    }
}
