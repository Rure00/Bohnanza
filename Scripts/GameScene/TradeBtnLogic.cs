using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class TradeBtnLogic : MonoBehaviour
{
    GameManager gameManagerLogic;
    PlayerLogic playerLogic;

    GameObject questionWindow;

    GameObject[] TradeBtn;

    public int btnIndex;
    int bellIndex;
    

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerLogic = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>();
        setBtn();

        gameObject.SetActive(false);
    }

    void setBtn()
    {
        //TradeButton Setting
        TradeBtn = new GameObject[3];
        questionWindow = gameObject.transform.GetChild(3).gameObject;

        for (int i =0; i <3; i++)
        {
            TradeBtn[i] = gameObject.transform.GetChild(i).gameObject;
            TradeBtn[i].AddComponent<TradeButton>();

            TradeBtn[i].GetComponent<TradeButton>().BtnIndex = i;
            TradeBtn[i].GetComponent<Button>().onClick.AddListener(TradeBtn[i].GetComponent<TradeButton>().tradebutton);
        }

        //Confirm, Cancel Button Setting
        questionWindow.transform.GetChild(0).GetChild(1).GetComponentInChildren<Button>().onClick.AddListener(ConfirmTrade);
        questionWindow.transform.GetChild(0).GetChild(2).GetComponentInChildren<Button>().onClick.AddListener(cancelTrade);
    }

    public void ConfirmTrade()
    {
        TradeBtn[btnIndex].GetComponent<TradeButton>().confirmTrade();
        gameManagerLogic.confirmSound.Play();
    }
    public void cancelTrade()
    {
        questionWindow.SetActive(false);
        //Turn On Farm Beans
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    playerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
            }
        }
        //Turn On Hand Beans
        for (int i = 0; i < 10; i++)
        {
            if (playerLogic.HandCard[9 - i])
                playerLogic.HandCard[9 - i].SetActive(true);
        }

        gameManagerLogic.denySound.Play();
    }

    void OnEnable()
    {
        bellIndex = playerLogic.bellIndex;
    }
}
