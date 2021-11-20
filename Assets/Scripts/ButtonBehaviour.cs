using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    [SerializeField] Sprite xSprite;
    [SerializeField] Sprite oSprite;
    [SerializeField] int posIndex;
    
    private ChessBoardManager manager;
    private Button button;
    
    void Start()
    {
        manager = FindObjectOfType<ChessBoardManager>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        if (manager.CanPlay)
        {
            if (!CheckIfOccupied())
            {
                ButtonUpdate(manager.PlayerID);
                manager.PlayerPlaceChess(posIndex, manager.PlayerID);
            }
        }
    }

    public void ButtonUpdate(int playerID)
    {
        if (playerID == 1)
        {
            button.GetComponent<Image>().sprite = oSprite;
        }
        else if (playerID == 2)
        {
            button.GetComponent<Image>().sprite = xSprite;
        }

        button.interactable = false;
    }

    bool CheckIfOccupied()
    {
        if (manager.ChessbordPos[posIndex] >= 1)
            return true;
        else
            return false;
    }
}
