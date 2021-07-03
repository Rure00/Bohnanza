using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

using Photon.Pun;
using Photon.Realtime;

public class CardData : MonoBehaviourPunCallbacks
{
    /*
     0 : 강낭콩
     1 : 팥콩
     2 : 동부콩
     3 : 대두콩
     4 : 완두콩
     5 : 똥콩
     6 : 칠리콩
     7 : 푸르대콩
     */

    SpriteRenderer cardSprite;
    public PlayerLogic playerLogic;

    public int cardIndex;
    public int number;

    public bool isOnCard;
    public bool isDragable = true;
    

    void Awake()
    {
        GenerateCompo();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isOnCard && (playerLogic.cardYouhave == null || playerLogic.cardYouhave == gameObject))
        {
            playerLogic.cardYouhave = gameObject;
            isDragable = true;
        }
        if(Input.GetMouseButtonDown(0) && isOnCard && playerLogic.canClick)
        {
            playerLogic.ClickCard(gameObject);
        }
    }

    public void GenerateCompo()
    {
        cardSprite = GetComponent<SpriteRenderer>();
        playerLogic = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>();
    }

    public void isTraded()
    {
        bool onTrade = false;

        for (int i =0; i < 5; i++)
        {
            if(playerLogic.mySuggest[i] != -1 && playerLogic.HandCard[playerLogic.mySuggest[i]] == gameObject)
            {
                onTrade = true;
                break;
            }
        }

        if(onTrade)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    void OnMouseEnter()
    {
        isOnCard = true;
        cardSprite.color = new Color(1, 1, 1, 0.4f);
    }
    void OnMouseExit()
    {
        isOnCard = false;
        cardSprite.color = new Color(1, 1, 1, 1);
    }
}
