using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class TradeWindowLogic : MonoBehaviourPunCallbacks
{
    GameManager gameManagerLogic;
    PlayerLogic playerLogic;

    public GameObject choiceWindow;

    public Transform orgPos;
    public Transform spreadPos;

    bool canMove;

    Rigidbody2D rigid;

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerLogic = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>();

        rigid = gameObject.GetComponent<Rigidbody2D>();

        choiceWindow = gameObject.transform.GetChild(8).gameObject;
        choiceWindow.SetActive(false);
    }

    void FixedUpdate()
    {
        if (canMove)
            moveWindow();
    }

    public void setNickName(int Bellindex)
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(true); // Turn on 'Change Button'.

        //Set NickNames.
        if (playerLogic.isMyTurn)
        { 
            gameObject.transform.GetChild(1).gameObject.SetActive(false); // Turn off 'Change Button'.
            // Turn on 'Trade Button Group'.
            gameObject.transform.GetChild(9).gameObject.SetActive(true);
            for(int i =0; i<3;i++)
            {
                if(i > PhotonNetwork.PlayerList.Length - 2)
                {
                    gameObject.transform.GetChild(9).GetChild(i).gameObject.SetActive(false);
                }
            }
            int a = 0;

            for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if(i != playerLogic.myIndex)
                {
                    gameObject.transform.GetChild(2 + a).GetComponentInChildren<Text>().text = gameManagerLogic.playerLogics[i].name;
                    a++;
                }
            }
        }
        else
        {
            int a = 1;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (i != playerLogic.myIndex && i != gameManagerLogic.curTurnPlayer.GetComponent<PlayerLogic>().myIndex)
                {
                    gameObject.transform.GetChild(2 + a).GetComponentInChildren<Text>().text = gameManagerLogic.playerLogics[i].name;
                    a++;
                }
                else if(i == playerLogic.myIndex && i != gameManagerLogic.curTurnPlayer.GetComponent<PlayerLogic>().myIndex)
                {
                    gameObject.transform.GetChild(2).GetComponentInChildren<Text>().text = playerLogic.photonView.Owner.NickName;
                }
            }
        }
    }
    public void setCard()
    {
        //Set Trade Cards.
        // mySugguestions : gameObject.transform.GetChild(5)
        // Sugguestions 1 : gameObject.transform.GetChild(6)
        // Sugguestions 2 : gameObject.transform.GetChild(7)

        //Deactive all cards.
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int dex = 0; dex < 8; dex++)
                {
                    gameObject.transform.GetChild(5 + index).GetChild(i).GetChild(dex).gameObject.SetActive(false);
                }
            }
        }

        //Active cards.
        if (playerLogic.isMyTurn)
        {
            int a = 0;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject suggestion = gameObject.transform.GetChild(5 + a).gameObject;

                if(i != playerLogic.myIndex)
                {
                    for (int index = 0; index < 5; index++)
                    {
                        int handIndex = gameManagerLogic.AllSuggestions[playerLogic.bellIndex, i, index];

                        if (handIndex != -1)
                        {
                            int beanIndex = gameManagerLogic.playerLogics[i].GetComponent<PlayerLogic>().HandCard[handIndex].GetComponent<CardData>().cardIndex;
                            suggestion.transform.GetChild(index).GetChild(beanIndex).gameObject.SetActive(true);
                        }
                    }

                    a++;
                }
            }
        }
        else
        {
            int a = 1;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject suggestion = gameObject.transform.GetChild(5 + a).gameObject;

                if(i != playerLogic.myIndex && i != gameManagerLogic.curTurnPlayer.GetComponent<PlayerLogic>().myIndex)
                {
                    for (int index = 0; index < 5; index++)
                    {
                        int handIndex = gameManagerLogic.AllSuggestions[playerLogic.bellIndex, i, index];

                        if (handIndex != -1)
                        {
                            int beanIndex = gameManagerLogic.playerLogics[i].GetComponent<PlayerLogic>().HandCard[handIndex].GetComponent<CardData>().cardIndex;
                            suggestion.transform.GetChild(index).GetChild(beanIndex).gameObject.SetActive(true);
                        }
                    }

                    a++;
                }
                else if(i == playerLogic.myIndex && i != gameManagerLogic.curTurnPlayer.GetComponent<PlayerLogic>().myIndex)
                {
                    for (int index = 0; index < 5; index++)
                    {
                        int handIndex = gameManagerLogic.AllSuggestions[playerLogic.bellIndex, i, index];

                        if (handIndex != -1)
                        {
                            int beanIndex = gameManagerLogic.playerLogics[i].GetComponent<PlayerLogic>().HandCard[handIndex].GetComponent<CardData>().cardIndex;
                            gameObject.transform.GetChild(5).gameObject.transform.GetChild(index).GetChild(beanIndex).gameObject.SetActive(true);
                        }
                    }
                }
                
            }
        }
    }

    public void changeMySuggestion()
    {
        //Allocated to 'Change Button'
        //Move up Cards on Trade.

        for (int i = 0; i < 5; i++)
        {
            if (playerLogic.mySuggest[i] != -1)
            {
                playerLogic.HandCard[playerLogic.mySuggest[i]].transform.position += Vector3.up * 0.5f;
            }
        }

        playerLogic.canClick = true;
        choiceWindow.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            playerLogic.mySuggest[i] = gameManagerLogic.AllSuggestions[playerLogic.bellIndex, playerLogic.myIndex, i];
        }

        playerLogic.SortingLayer();

        gameManagerLogic.clickSound.Play();
        CloseWindow();
    }

    public void ConfirmSuggest()
    {
        playerLogic.canClick = false;
        choiceWindow.SetActive(false);

        //트레이드된 카드 원래 위치로 옮기기
        for(int i = 0; i< 5; i++)
        {
            if(playerLogic.mySuggest[i] != -1)
            {
                playerLogic.HandCard[playerLogic.mySuggest[i]].transform.position += Vector3.down * 0.5f;
            }
        }

        playerLogic.photonView.RPC("RPConSuggest", RpcTarget.All, playerLogic.mySuggest, playerLogic.bellIndex);

        if (playerLogic.photonView.IsMine && playerLogic.isMyTurn)
        {
            int[] opSuggestion = new int[5];
            playerLogic.photonView.RPC("RPConSuggest", RpcTarget.All, playerLogic.mySuggest, playerLogic.bellIndex);

            for (int i =0; i< 10; i++)
            {
                if (playerLogic.HandCard[i])
                    playerLogic.HandCard[i].SetActive(false);
            }
            for(int i =0; i< 5; i++)
            {
                opSuggestion[i] = gameManagerLogic.AllSuggestions[playerLogic.bellIndex, playerLogic.SuggestedPlayerIndex, i];
            } 

            if (playerLogic.photonView.IsMine)
            {
                playerLogic.photonView.RPC("HandOverCard", RpcTarget.All, playerLogic.SuggestedPlayerIndex, opSuggestion);
            }
        }

        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
                else
                    break;
            }
        }

        if (gameManagerLogic.AllSuggestions[playerLogic.bellIndex, playerLogic.SuggestedPlayerIndex, 0] == -1)
        {
            playerLogic.photonView.RPC("EndSuggestedBeansPlanting", RpcTarget.All, false, false);
        }

        gameManagerLogic.confirmSound.Play();
    }

    public void CloseWindow()
    {
        //Active Cards
        for (int i = 0; i < 10; i++)
        {
            if (playerLogic.HandCard[9 - i])
            {
                playerLogic.HandCard[9 - i].SetActive(true);
            }
        }
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
                else
                    break;
            }
        }

        gameManagerLogic.clickSound.Play();
        canMove = true;
    }

    void moveWindow()
    {
        rigid.AddForce(Vector3.right, ForceMode2D.Impulse);

        if (Mathf.Abs(transform.position.x - orgPos.position.x) < 0.5f)
        {
            transform.position = orgPos.position;
            rigid.velocity = Vector3.zero;
            canMove = false;
        }
    }
}
