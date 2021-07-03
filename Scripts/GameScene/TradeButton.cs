using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class TradeButton : MonoBehaviour
{
    GameManager gameManagerLogic;
    PlayerLogic playerLogic;

    public int BtnIndex;
    public int[] suggestion;
    int SuggestedPlayerIndex;



    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerLogic = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>();

        suggestion = new int[5];

    }

    public void tradebutton()
    {
        gameObject.transform.parent.gameObject.GetComponent<TradeBtnLogic>().btnIndex = BtnIndex;
        SuggestedPlayerIndex = getPlayerIndex();

        for (int i = 0; i < 5; i++)
        {
            suggestion[i] = gameManagerLogic.AllSuggestions[playerLogic.bellIndex, SuggestedPlayerIndex, i];
        }

        //Close Trade Window.
        gameObject.transform.parent.parent.GetComponent<TradeWindowLogic>().CloseWindow();
        gameObject.transform.parent.GetChild(3).gameObject.SetActive(true);

        //Turn Off Cards.
        for(int i =0; i< 10; i++)
        {
            if (playerLogic.HandCard[i])
                playerLogic.HandCard[i].SetActive(false);
        }
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(false);
                else
                    break;
            }
        }

        gameManagerLogic.clickSound.Play();
    }
    public int getPlayerIndex()
    {
        int playerIndex = 0;

        int num = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i != playerLogic.myIndex)
            {
                if (BtnIndex == num)
                {
                    playerIndex = i;
                    break;
                }
                num++;
            }
        }

        return playerIndex;
    }

    public void confirmTrade()
    {
        //      *Turn off Question Window*  //
        gameObject.transform.parent.GetChild(3).gameObject.SetActive(false);

        //      *Show Suggested Beans.*     //
        PlayerLogic tradeOpponent = gameManagerLogic.playerLogics[SuggestedPlayerIndex].GetComponent<PlayerLogic>();
        playerLogic.SuggestedPlayerIndex = SuggestedPlayerIndex;

        playerLogic.isTrading = true;

        //  *Turn On Farm Beans.*     //
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
        //  *Turn Off Hand Beans*   //
        for (int i = 0; i < 10; i++)
        {
            if (playerLogic.HandCard[9 - i])
                playerLogic.HandCard[9 - i].SetActive(true);
            else
                break;
        }

        gameManagerLogic.confirmSound.Play();
        gameObject.transform.parent.parent.GetComponent<TradeWindowLogic>().changeMySuggestion();
    }
}
