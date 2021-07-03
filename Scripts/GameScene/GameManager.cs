using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

/* Rule!!
 
 * myTurn
  On my turn,
  First, at least Plant 1 bean. Can plant 2 beans.
  Second, Draw 2 Market Cards and Trade.
  Third, Draw 3 Cards.

 * Whenever I want
  Selling Bean.

 * Whene sell beans, this Bean Card should be removed.
  and next Pile of Deck will be started with less cards.

*/

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject cameraManager;
    public GameObject waitUI;

    public GameObject[] playerLogics;
    public GameObject[] PlayerTurn;
    public GameObject[] cards;
    public GameObject[] allUI;
    public GameObject[] Deck;
    public GameObject[] abandonedDeck;
    public GameObject[] marketBean;
    public GameObject[] BlockTouch;
    public GameObject ResultWindow;


    public GameObject ExitWindow;
    public GameObject curTurnPlayer;

    public bool[] playerReady;

    public int DeckNumb;
    public int givenTime;
    public int orderIndex;
    public int DeckRound;
    int LocalIndex;

    public int[,,] AllSuggestions;   //[card wanted to be traded, who suggest, what cards suggested]

    public AudioSource BGM;
    public AudioSource getPointSound; // 28
    public AudioSource denySound; // 29
    public AudioSource confirmSound; // 15
    public AudioSource clickSound; // 31
    public AudioSource plantSound; // 41


    void Awake()
    {
        playerReady = new bool[4];
        playerReady[0] = true;
        playerLogics = new GameObject[4];
        PlayerTurn = new GameObject[4];
        Deck = new GameObject[104];
        abandonedDeck = new GameObject[104];
        marketBean = new GameObject[2];

        StartCoroutine(PlayBGM());
    }

    #region OnWaitUI

    IEnumerator PlayBGM()
    {
        BGM.Play();

        yield return new WaitForSeconds(110f);

        StartCoroutine(PlayBGM());
    }

    IEnumerator SetMyCamreaIndex()
    {
        yield return new WaitForSeconds(0.5f);

        cameraManager.GetComponent<CameraLogic>().MyIndex 
            = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>().myIndex;
    }

    public void makeCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            CardData cardData = cards[i].GetComponent<CardData>();

            for (int dex = 0; dex < cardData.number; dex++)
            {
                Deck[DeckNumb] = Instantiate(cards[i], Vector3.zero, Quaternion.identity);
                DeckNumb++;
            }
        }
    }

    public void ShowExitRoomWindow()
    {
        ExitWindow.SetActive(true);
    }
    public void exitConfirm()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
    public void exitCancel()
    {
        ExitWindow.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        GameObject newplayer = PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
        PlayerLogic newplLogic = newplayer.GetComponent<PlayerLogic>();

        if(newplLogic.photonView.IsMine)
        {
            newplLogic.photonView.RPC("SetplayerLogics", RpcTarget.AllBufferedViaServer, null);
            newplLogic.photonView.RPC("getMyIndex", RpcTarget.AllBufferedViaServer, null);
            newplLogic.photonView.RPC("getMyNicKName", RpcTarget.AllBufferedViaServer, null); 
        }

        if (PhotonNetwork.IsMasterClient)
        {
            newplLogic.startBtn = waitUI.GetComponent<WaitUILogic>().startBtn;
            newplLogic.startBtn.SetActive(true);
            newplLogic.startBtn.GetComponent<Button>().onClick.AddListener(newplLogic.StartGameBtn);
        }
        else
        {
            newplLogic.readyBtn = waitUI.GetComponent<WaitUILogic>().readyBtn;
            newplLogic.readyBtn.SetActive(true);
            newplLogic.readyBtn.GetComponent<Button>().onClick.AddListener(newplLogic.ReadyBtn);

            newplLogic.cancelBtn = waitUI.GetComponent<WaitUILogic>().cancelBtn;
        }

        StartCoroutine(SetMyCamreaIndex());
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.LoadLevel(1);

        Debug.Log("Creating Room Failed");
    }

    #endregion

    #region InGame

    /*
    Start Game by MasterClinet's RPC - Includes GameManager.PlayGame()
    TurningOrder() - use 'for' function. Includes all player's actions.
     */

    public void SortingAllHand()
    {
        for(int i = 0; i<PhotonNetwork.PlayerList.Length; i++)
        {
            playerLogics[i].GetComponent<PlayerLogic>().photonView.RPC("SortingHand", RpcTarget.All, null);
        }
    }

    public void PlayGame()
    {
        getLocalIndex();
        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            BlockTouch[i].SetActive(true);
        }

        if (PhotonNetwork.IsMasterClient)
            SetTurn();

        cameraManager.GetComponent<CameraLogic>().SetNickName();

        //When Game Starts, Each Players Draw 5 Cards.
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                PhotonView PV = playerLogics[i].GetComponent<PlayerLogic>().photonView;

                int[] randomNums = playerLogics[0].GetComponent<PlayerLogic>().getRandomIndex(5, DeckNumb, true);

                PV.RPC("DrawCard", RpcTarget.All, randomNums, i);
            }
        }

        StartCoroutine(TurningOrder());
    }

    void getLocalIndex()
    {
        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i])
            {
                LocalIndex = i;
                return;
            }
        }
    }

    void SetTurn()
    {
        PlayerLogic MasterPL = playerLogics[0].GetComponent<PlayerLogic>();

        int[] randomNums = MasterPL.getRandomIndex(PhotonNetwork.PlayerList.Length, PhotonNetwork.PlayerList.Length, false);
        
        if(orderIndex == 1)
        {
            for(int i =0; i< PhotonNetwork.PlayerList.Length;i++)
            {
                if(randomNums[i] == 0)
                {
                    int alter = randomNums[0];
                    randomNums[0] = 0;
                    randomNums[i] = alter;
                    break;
                }
            }
        }
        else if(orderIndex == 2)
        {
            if(randomNums[0] == 0)
            {
                int alter = randomNums[1];
                randomNums[1] = 0;
                randomNums[0] = alter;
            }
        }

        MasterPL.photonView.RPC("SetGMvariable", RpcTarget.All, randomNums);
    }

    IEnumerator TurningOrder()
    {
        yield return new WaitForSeconds(1.0f);

        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            curTurnPlayer = PlayerTurn[i];


            curTurnPlayer.GetComponent<PlayerLogic>().isMyTurn = true;
            curTurnPlayer.GetComponent<PlayerLogic>().plantingNum = 0;

            yield return StartCoroutine(myTurnAction(curTurnPlayer));

            //Show Panel
            PlayerTurn[i].GetComponent<PlayerLogic>().isMyTurn = false;
        }

        yield return null;

        if(DeckRound == 3)
        {
            //Show Result Window
            for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
            {
                allUI[i].SetActive(true);
            }
            cameraManager.GetComponent<CameraLogic>().GetFirstCamera();

            ResultWindow.SetActive(true);
            ResultWindow.GetComponent<ResultUILogic>().ShowResult();
        }
        else
        {
            StartCoroutine(TurningOrder());
        }
    }
    IEnumerator myTurnAction(GameObject turnPlayerLogic)
    {
        PlayerLogic MasterPL = playerLogics[0].GetComponent<PlayerLogic>();
        PlayerLogic turnPL = turnPlayerLogic.GetComponent<PlayerLogic>();

        //Plant My HandBeans.
        /*    Just Adding bool 'canPlant' is Enough.    */
        SortingAllHand();
        yield return StartCoroutine(showPanels(1.6f, curTurnPlayer.name + "의 턴! : " + "\n" + "콩을 심으세요!"));

        if (turnPL.photonView.IsMine)
            turnPL.photonView.RPC("PlantBeans", RpcTarget.All, null);

        givenTime = 60;
        yield return StartCoroutine(StopWatch(0));
        SortingAllPlayerHand();

        //Get Market Cards.
        if (PhotonNetwork.IsMasterClient)
        {
            int[] marketBeanIndex = MasterPL.getRandomIndex(2, DeckNumb, false);
            MasterPL.photonView.RPC("DrawMarketCard", RpcTarget.All, marketBeanIndex);
        }

        SortingAllHand();
        StopCoroutine("showPanels");
        yield return StartCoroutine(showPanels(1.6f, curTurnPlayer.name + "의 턴! : " + "\n" + "콩을 거래하세요!"));
        SortingAllPlayerHand();

        //Trade.
        setTradeBool(true);

        givenTime = 90;
        yield return StartCoroutine(StopWatch(-1));

        SortingAllHand();
        //Plant left Market beans(If Not Left, Skip).
        if (marketBean[0] || marketBean[1])
        {
            turnPL.canPlant = true;
            turnPL.isTrading = true;

            //Clean Off Market
            for (int UIindex = 0; UIindex < 4; UIindex++)
            {
                for (int i = 0; i < 8; i++)
                {
                    allUI[UIindex].transform.GetChild(5).GetChild(1).GetChild(i).gameObject.SetActive(false);
                    allUI[UIindex].transform.GetChild(5).GetChild(2).GetChild(i).gameObject.SetActive(false);
                }
            }

            //Fill Hand With Market Beans.
            for (int i = 0; i < 10; i++)
            {
                if (turnPL.HandCard[i])
                {
                    turnPL.alterHandCards[i] = turnPL.HandCard[i];
                    turnPL.alterHandCards[i].SetActive(false);
                    turnPL.HandCard[i] = null;
                }
            }

            int handnum = 0;
            for (int i = 0; i < 2; i++)
            {
                if (marketBean[i])
                {
                    turnPL.HandCard[handnum] = marketBean[i];
                    marketBean[i].transform.position = turnPL.cardPos[handnum].position;
                    turnPL.photonView.RPC("SortingHand", RpcTarget.All, null);
                    handnum++;
                }
            }

            givenTime = 30;
            yield return StartCoroutine(StopWatch(1));
            SortingAllPlayerHand();

            turnPL.isTrading = false;
        }

        //Draw 3 Cards.
        SortingAllHand();
        StopCoroutine("showPanels");
        yield return StartCoroutine(showPanels(1.6f, curTurnPlayer.name + "의 턴! : " + "\n" + "콩을 세장 뽑습니다!"));
        int num = 3;
        if(DeckNumb < 3)
        {
            num = 2;
            DeckRound++;
        }

        if(MasterPL.photonView.IsMine)
        {
            int[] randNum = MasterPL.getRandomIndex(num, DeckNumb, false);
            Debug.Log(turnPL.name + " Draw!");
            turnPL.photonView.RPC("DrawCard", RpcTarget.All, randNum, turnPL.myIndex);
        }

        yield return null;
    }
    void setTradeBool(bool can)
    {
        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerLogics[i].GetComponent<PlayerLogic>().canTrade = can;
        }
    }
    public void getSuggest(int bellindex, int playerIndex, int[] suggestion)
    {
        for(int i = 0; i < 5; i++)
        {
            AllSuggestions[bellindex, playerIndex, i] =  suggestion[i];
        }
    }
    public void RemoveSuggestion(int bellindex)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            for (int index = 0; index < 5; index++)
            {
                AllSuggestions[bellindex, i, index] = -1;
            }
        }

        marketBean[bellindex] = null;
    }

    void SortingAllPlayerHand()
    {
        for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            playerLogics[i].GetComponent<PlayerLogic>().photonView.RPC("SortingHand", RpcTarget.All, null);
        }
    }

    #endregion

    #region UI

    public IEnumerator StopWatch(int index)
    {
        WaitForSeconds watiforSecond = new WaitForSeconds(1.0f);

        while (givenTime > 0)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                allUI[i].transform.GetChild(10).GetComponentInChildren<Text>().text = givenTime.ToString();
            }

            yield return watiforSecond;

            givenTime--;
        }

        if (givenTime == 0 || givenTime == -1)
        {
            switch (index)
            {
                case 0:
                    //Plant my Beans.
                    curTurnPlayer.GetComponent<PlayerLogic>().canPlant = false;
                    break;
                case 1:
                    //Trade.
                    setTradeBool(false);
                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        playerLogics[i].GetComponent<PlayerLogic>().photonView.RPC("EndSuggestedBeansPlanting", RpcTarget.All, true, true);
                    }
                    break;
                default:
                    Debug.Log("Default case");
                    break;
            }
        }
        else
        {
            Debug.Log(" error : Left sec is " + givenTime);
        }
    }

    IEnumerator showPanels(float time, string msg)
    {
        BlockTouch[LocalIndex].SetActive(true);
        BlockTouch[LocalIndex].transform.GetChild(1).gameObject.SetActive(false);
        BlockTouch[LocalIndex].transform.GetChild(2).gameObject.SetActive(true);

        for (int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            StartCoroutine(playerLogics[i].GetComponent<PlayerLogic>().ShowPanel(time, msg));
        }

        yield return new WaitForSeconds(time + 2);

        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            BlockTouch[i].transform.GetChild(2).gameObject.SetActive(false);
        }

        BlockTouch[LocalIndex].SetActive(false);
    }

    #endregion
}
