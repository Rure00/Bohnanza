using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellButtonLogic : MonoBehaviour
{
    TradeWindowLogic tradeWindowLogic;
    GameManager gameManagerLogic;
    PlayerLogic playerLogic;

    GameObject TradeWindow;
    public int BellIndex;

    bool canMove;

    Rigidbody2D rigid;

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        playerLogic = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>();

        TradeWindow = gameManagerLogic.allUI[playerLogic.myIndex].transform.GetChild(11).gameObject;
        tradeWindowLogic = TradeWindow.GetComponent<TradeWindowLogic>();
        rigid = TradeWindow.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(canMove)
        {
            MovdWindow();
        }
    }

    void getMyPlayerLogic()
    {
        for (int i = 0; i < 4; i++)
        {
            if (gameManagerLogic.playerLogics[i])
            {
                PlayerLogic PL = gameManagerLogic.playerLogics[i].GetComponent<PlayerLogic>();

                if (PL.photonView.IsMine)
                    playerLogic = PL;
                return;
            }
        }
    }

    public void SpreadWindow()
    {
        if (playerLogic.canTrade && !playerLogic.isTrading && gameManagerLogic.marketBean[BellIndex])
        {
            playerLogic.bellIndex = BellIndex;
            tradeWindowLogic.setNickName(BellIndex);
            tradeWindowLogic.setCard();

            for (int i = 0; i < 5; i++)
            {
                playerLogic.mySuggest[i] = gameManagerLogic.AllSuggestions[playerLogic.SuggestedPlayerIndex, playerLogic.myIndex, i];
            }

            //Turn Off Cards.
            for (int i = 0; i < playerLogic.HandCard.Length; i++)
            {
                if (playerLogic.HandCard[i])
                    playerLogic.HandCard[i].SetActive(false);
                else
                    break;
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
            canMove = true;
        }
        else if(!gameManagerLogic.marketBean[BellIndex])
        {
            Debug.Log("There is no Market Bean.");
        }
        else
        {
            Debug.Log("Can't Spread Trade Window");
        }
    }

    void MovdWindow()
    {
        rigid.AddForce(Vector3.left, ForceMode2D.Impulse);
        
        if(Mathf.Abs(TradeWindow.transform.position.x - tradeWindowLogic.spreadPos.position.x) < 0.5f)
        {
            TradeWindow.transform.position = tradeWindowLogic.spreadPos.position;
            rigid.velocity = Vector3.zero;
            canMove = false;
        }
    }
    
}
