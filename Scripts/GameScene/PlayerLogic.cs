using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class PlayerLogic : MonoBehaviourPunCallbacks
{
    GameManager gameManagerLogic;
    CameraLogic cameraManagerLogic;
    PhotonView PV;
    CardData cardData;
    DropZoneLogic dropZoneLogic;

    public GameObject startBtn;
    public GameObject readyBtn;
    public GameObject cancelBtn;
    public GameObject myUI;
    public GameObject cardYouhave;
    public GameObject farmYouChoose;

    public GameObject[] HandCard;
    public GameObject[] alterHandCards;

    public GameObject ActionButton;
    public GameObject tradeWindow;

    GameObject myBlockTouch;

    public bool isReady = false;
    public bool isMyTurn;
    public bool canPlant;
    public bool canTrade;
    public bool canClick;
    public bool isTrading;
    public bool canSell;
    bool didBuy;

    public int[] mySuggest; // Indexes are in HandCard

    public int myIndex;
    public int bellIndex;
    public int sugPlantingNum;
    public int SuggestedPlayerIndex;
    public int myScore;
    int handcardNum;
    public int plantingNum;
    public int farmIndex;

    public Transform[] cardPos;

    public Vector3 orgPos;

    void Awake()
    {
        PV = this.GetComponent<PhotonView>();
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        cameraManagerLogic = GameObject.FindWithTag("CameraManager").GetComponent<CameraLogic>();

        if(PV.IsMine)
        {
            gameObject.name = PhotonNetwork.NickName;
            //Set NickName panel in myUI.
        }

        HandCard = new GameObject[10];
        alterHandCards = new GameObject[10];
        cardPos = new Transform[10];
        mySuggest = new int[5] {-1,-1,-1,-1,-1};

        GameObject.FindWithTag("VoiceManager").GetComponent<VoiceManagerLogic>().getID();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && cardYouhave != null && PV.IsMine)
        {
            orgPos = cardYouhave.transform.position;
        }
        if (Input.GetMouseButton(0) && PV.IsMine && canPlant)
        {
            DragCard();
        }
        if ((cardYouhave != null) && PV.IsMine && canPlant)
        {
            DropCard();
        }
        if(Input.GetMouseButton(0) && PV.IsMine && canSell)
        {
            SellBeans();
        }
    }

    #region OnWait
    [PunRPC]
    void getMyNicKName()
    {
        gameObject.name = PV.Owner.NickName;
    }

    public void StartGameBtn()
    {
        if (CheckAllplayerReady())
        {
            //게임 시작.

            if(PV.IsMine)
            {
                PV.RPC("turnOffWaitUI", RpcTarget.All, null);
            }

            PV.RPC("getMyCamera", RpcTarget.All, null);
            PV.RPC("SetCards", RpcTarget.All, null);

            PV.RPC("PlayGame", RpcTarget.All, null);
        }
        else
        {
            //플레이어 상태를 확인해주세요.
            Debug.Log("At leat, Need two players or Not all players are ready");
        }
    }


    public void ReadyBtn()
    {
        //게임 준비.
        if(PV.IsMine)
        {
            PV.RPC("turnOnCheck", RpcTarget.AllBuffered, true);
        }

        cancelBtn.SetActive(true);
        cancelBtn.GetComponent<Button>().onClick.AddListener(CancelReadyBtn);

        readyBtn.SetActive(false);
    }

    public void CancelReadyBtn()
    {
        //준비 취소.
       if(PV.IsMine)
        {
            PV.RPC("turnOnCheck", RpcTarget.AllBuffered, false);
        }

        readyBtn.SetActive(true);
        cancelBtn.SetActive(false); 
    }

    bool CheckAllplayerReady()
    {
        bool canStart = true;
        //플레이어 준비 확인
        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!gameManagerLogic.playerReady[i])
            {
                canStart = false;
            }
        }
        //플레이어 수 확인
        if(PhotonNetwork.PlayerList.Length < 2)
        {
            canStart = false;
        }

        return canStart;
    }
    [PunRPC]
    void turnOffWaitUI()
    {
        //PlayerLogic : StartGameBtn
        GameObject.FindWithTag("WaitUI").SetActive(false);

        for(int i =0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            gameManagerLogic.allUI[i].SetActive(true);
        }
    }
    [PunRPC]
    void turnOnCheck(bool isReady)
    {
        //PlayerLogic : ReadyBtn

        WaitUILogic waitUILogic = GameObject.FindWithTag("WaitUI").GetComponent<WaitUILogic>();
        GameObject myCheck = waitUILogic.NameBox[myIndex].transform.GetChild(2).gameObject;

        if (isReady)
        {
            myCheck.SetActive(true);
            gameManagerLogic.playerReady[myIndex] = true;
        }
        else
        {
            myCheck.SetActive(false);
            gameManagerLogic.playerReady[myIndex] = false;
        }
    }

    [PunRPC]
    void getMyIndex()
    {
        //GameManager : OnJoinedRoom
        for (int i = 0; i < 4; i++)
        {
            if (gameManagerLogic.playerLogics[i] == gameObject)
            {
                myIndex = i;

                if (PV.IsMine)
                    PV.RPC("SetMyUI", RpcTarget.AllBuffered, null);

                return;
            }
        }
    }
    [PunRPC]
    void SetplayerLogics()
    {
        //GameManager : OnJoinedRoom
        for(int i =0; i<4;i++)
        {
            if(PhotonNetwork.PlayerList[i] == PV.Owner)
            {
                gameManagerLogic.playerLogics[i] = gameObject;
                return;
            }
        }
    }
    [PunRPC]
    void SetCards()
    {
        //PlayerLogic : StartBtn
        gameManagerLogic.DeckNumb = 0;

        gameManagerLogic.makeCards();

        for (int i =0; i< 104; i++)
        {
            gameManagerLogic.Deck[i].SetActive(false);
        }

        gameManagerLogic.AllSuggestions = new int[2, PhotonNetwork.PlayerList.Length, 5];
        //Initializate AllSuggestions[].
        for (int i = 0; i < 2; i++)
        {
            for(int index =0; index < PhotonNetwork.PlayerList.Length;index++)
            {
                for(int dex = 0; dex < 5; dex ++)
                {
                    gameManagerLogic.AllSuggestions[i, index, dex] = -1;
                }
            }
        }   
    }
    [PunRPC]
    void PlayGame()
    {
        gameManagerLogic.PlayGame();
    }

    #endregion

    #region MasterClient

    [PunRPC]
    public void DrawCard(int[] randomNums, int playerIndex)
    {
        //GameManager : PlayGame
        //GameManager : myTurnAction
        int num = randomNums.Length;
        handcardNum = 0;

        for (int i =0; i < 9; i++)
        {
            if (HandCard[i])
                handcardNum++;
        }
        if(num > 10 - handcardNum)
        {
            num = 10 - handcardNum;
        }

        for (int index = 0; index < num; index++)
        {
            //get card by random index.
            HandCard[handcardNum] = gameManagerLogic.Deck[randomNums[index]];
            //Sorting Deck.
            gameManagerLogic.Deck[randomNums[index]] = gameManagerLogic.Deck[gameManagerLogic.DeckNumb - 1];
            //Remove Deck[ranNum] from deck.
            gameManagerLogic.Deck[gameManagerLogic.DeckNumb - 1] = null;

            HandCard[handcardNum].SetActive(true);
            HandCard[handcardNum].GetComponent<CardData>().playerLogic = gameObject.GetComponent<PlayerLogic>();

            gameManagerLogic.DeckNumb--;
            handcardNum++;
        }

        PV.RPC("SortingHand", RpcTarget.All, null);
    }
    public int[] getRandomIndex(int num, int range, bool descending)
    {
        int[] randomNums = new int[num];

        int id = 0;
        while(id < num)
        {
            bool isOverlap = false;
            randomNums[id] = Random.Range(0, range);

            for(int dex = 0; dex < id; dex++)
            {
                if(randomNums[id] == randomNums[dex])
                {
                    isOverlap = true;
                }
            }

            if(!isOverlap)
                id++;
        }

        //descending randomNums[]
        if(descending)
        {
            int a = num - 1;
            for (int i = 0; i < a; i++)
            {
                int b = num - i;
                for (int index = 0; index < b; index++)
                {
                    if (randomNums[i] < randomNums[i + index])
                    {
                        int alter = randomNums[i];
                        randomNums[i] = randomNums[i + index];
                        randomNums[i + index] = alter;
                    }
                }

            }
        }

        return randomNums;
    }
    [PunRPC]
    public void DrawMarketCard(int[] randomNums)
    {
        //GameManager : myTurnAction

        //Get Beans Index
        //Set Active Market Cards.

        gameManagerLogic.marketBean[0] = gameManagerLogic.Deck[randomNums[0]];
        gameManagerLogic.marketBean[1] = gameManagerLogic.Deck[randomNums[1]];

        gameManagerLogic.Deck[randomNums[0]] = null;
        gameManagerLogic.Deck[randomNums[1]] = null;

        gameManagerLogic.Deck[randomNums[0]] = gameManagerLogic.Deck[gameManagerLogic.DeckNumb - 1];
        gameManagerLogic.Deck[gameManagerLogic.DeckNumb - 1] = null;
        gameManagerLogic.Deck[randomNums[1]] = gameManagerLogic.Deck[gameManagerLogic.DeckNumb - 2];
        gameManagerLogic.Deck[gameManagerLogic.DeckNumb - 2] = null;

        gameManagerLogic.DeckNumb -= 2;

        int[] beanindexes = new int[2];
        beanindexes[0] = gameManagerLogic.marketBean[0].GetComponent<CardData>().cardIndex;
        beanindexes[1] = gameManagerLogic.marketBean[1].GetComponent<CardData>().cardIndex;

        for (int UIindex = 0; UIindex < 4; UIindex++)
        {
            gameManagerLogic.allUI[UIindex].transform.GetChild(5).GetChild(1).GetChild(beanindexes[0]).gameObject.SetActive(true);
            gameManagerLogic.allUI[UIindex].transform.GetChild(5).GetChild(2).GetChild(beanindexes[1]).gameObject.SetActive(true);
        }
    }
    [PunRPC]
    void SetGMvariable(int[] randNums)
    {
        //GameManager : SetTurn
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            gameManagerLogic.PlayerTurn[i] = gameManagerLogic.playerLogics[randNums[i]];
        }
    }

    #endregion

    #region InGame

    [PunRPC]
    public void SortingHand()
    {
        //PlayerLogic : DrawCard
        //PlayerLogic : DropCard
        //PlayerLogic : EndSuggestedBeansPlanting

        handcardNum = 0;
        for (int i = 0; i < 10; i++)
        {
            if (HandCard[i])
                handcardNum++;
        }

        for (int i = 0; i < 10; i++)
        {
            if(HandCard[i] == null && i < 9)
            {
                int index = i;
                while (HandCard[index] == null && index != 9)
                {
                    index++;
                }

                if(HandCard[index] != null)
                {
                    HandCard[i] = HandCard[index];
                    HandCard[index] = null;
                }
            }
        }
        
        int a = handcardNum - 1;
        if(handcardNum != 0)
        {
            for (int i = 0; i < handcardNum; i++)
            {
                HandCard[i].transform.position = cardPos[a - i].transform.position;
            }
        }

        if (HandCard[0])
        {
            HandCard[0].GetComponent<BoxCollider2D>().size = new Vector2(1, 1.28f);
            HandCard[0].GetComponent<BoxCollider2D>().offset = Vector2.zero;
        }

        sortingOrderInLayer();
    }

    void sortingOrderInLayer()
    {
        for(int i =0; i< 10; i++)
        {
            if(HandCard[i])
            {
                HandCard[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = 10 - i;
                HandCard[i].SetActive(true);
            }
        }
    }

    void DragCard()
    {
        if(cardYouhave == null)
        {
           // Debug.Log("Error : " + 1);
        }
        if (!isTrading)
        {
            //Debug.Log("Error : " + 2);
        }


        if (cardYouhave == null ||(cardYouhave != HandCard[0] && !isTrading))
        {
            //Debug.Log("Error : " + 3);
            return;
        }

        cardData = cardYouhave.GetComponent<CardData>();
        GameObject myCamera = cameraManagerLogic.cameras[myIndex];

        if (cardData.isDragable)
        {
            Vector3 mousePos = myCamera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -myCamera.transform.position.z));

            cardYouhave.transform.position = mousePos;

            cardYouhave.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    void DropCard()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (farmYouChoose != null)
            {
                // farm에 종속 : 팜의 beanIndex, beanNumb 바꿔주고 handCard에서 빼기
                dropZoneLogic = farmYouChoose.GetComponent<DropZoneLogic>();

                int cardIndex = -1;
                for(int i =0; i< 10; i++)
                {
                    if(HandCard[i] == cardYouhave)
                    {
                        cardIndex = i;
                        break;
                    }
                }

                if ((dropZoneLogic.beanIndex == cardData.cardIndex || dropZoneLogic.beanIndex == -1) && canPlant && dropZoneLogic.isBought)
                {   //Drop Success
                    int farmindex = dropZoneLogic.farmIndex;

                    if (PV.IsMine)
                    {
                        PV.RPC("sendCardIndex", RpcTarget.All, cardIndex);
                        PV.RPC("RPConDropCard", RpcTarget.All, farmindex);

                        if(cardYouhave)
                            dropZoneLogic.Scoring(cardYouhave.GetComponent<CardData>().cardIndex);

                        plantingNum++;

                        if(plantingNum == 2 && !canTrade)
                        {
                            PV.RPC("skipLeftTime", RpcTarget.All, null);
                        }
                    }

                    gameManagerLogic.plantSound.Play();
                }
                else //Drop Fail
                {
                    cardYouhave.GetComponent<BoxCollider2D>().enabled = true;
                    cardYouhave.transform.position = orgPos;

                    gameManagerLogic.denySound.Play();
                }
            }
            else if (farmYouChoose == null)
            {
                Debug.Log("Drop disable : farmYouChoose is null");
                cardYouhave.GetComponent<BoxCollider2D>().enabled = true;
                cardYouhave.transform.position = orgPos;

                gameManagerLogic.denySound.Play();
            }

            if(isTrading)
            {
                if(handcardNum == 0 && PV.IsMine)
                {
                    Debug.Log("Played");
                    PV.RPC("EndSuggestedBeansPlanting", RpcTarget.All, false, false);
                }
            }

            handcardNum = 0;
            for(int i = 0; i<10; i++)
            {
                if (HandCard[i])
                    handcardNum++;
            }
            if(handcardNum == 0)
            {
                bool isAlterEmpty = true;

                for(int i = 0; i< 10; i++)
                {
                    if (alterHandCards[i])
                    {
                        isAlterEmpty = false;
                        break;
                    }
                }

                if(!isAlterEmpty)
                {
                    PV.RPC("EndSuggestedBeansPlanting", RpcTarget.All, false, false);
                }
            }


            gameManagerLogic.SortingAllHand();

            cardYouhave = null;
        }
    }
    [PunRPC]
    void EndSuggestedBeansPlanting(bool isAway, bool isOnMarket)
    {
        //PlayerLogic : DropCard
        //GameManager : StopWatch

        bool isHandFill = false;
        for (int i = 0; i < 10; i++)
        {
            if (HandCard[i])
            {
                isHandFill = true;
                break;
            }
        }

        tradeWindow.GetComponent<TradeWindowLogic>().CloseWindow();

        for (int i =0; i<10; i++)
        {
            //Find Empty Index in abandonedDeck.
            int EmptyIndex = 0;
            while(gameManagerLogic.abandonedDeck[EmptyIndex])
            {
                EmptyIndex++;
            }

            if (isAway && HandCard[i])
            {
                gameManagerLogic.abandonedDeck[EmptyIndex] = HandCard[i];
                gameManagerLogic.abandonedDeck[EmptyIndex].SetActive(false);
            }

            if (!isHandFill && isOnMarket)
            {
                gameManagerLogic.givenTime = 0;
            }

            if (alterHandCards[i])
            {
                HandCard[i] = alterHandCards[i];
                alterHandCards[i] = null;
            }
        }

        for(int i =0; i < 10; i++)
        {
            if (HandCard[9 - i])
                HandCard[9 - i].SetActive(true);
        }

        canPlant = false;
        isTrading = false;

        PV.RPC("SortingHand", RpcTarget.All, null);
    }
    [PunRPC]
    void sendCardIndex(int cardIndex)
    {
        //PlayerLogic : DropCard
        if(cardIndex == -1)
        {
            Debug.Log("Error : cardIndex is -1");
        }
        else
        {
            cardYouhave = HandCard[cardIndex];
        }

        if(cardIndex == -1)
        {
            cardYouhave = null;
        }
    }
    [PunRPC]
    void RPConDropCard(int farmIndex)
    {
        //PlayerLogic : DropCard

        if (!cardYouhave)
            return;

        CardData cardData = cardYouhave.GetComponent<CardData>();
        GameObject myPlantingPos = myUI.transform.GetChild(5).GetChild(6).gameObject;
        dropZoneLogic = myPlantingPos.transform.GetChild(farmIndex).GetComponent<DropZoneLogic>();

        dropZoneLogic.beanIndex = cardData.cardIndex;
        dropZoneLogic.beanNumb++;

        //Get empty index in plantedBeans.
        int lastIndex = 0;
        for (int i = 0; i < 15; i++)
        {
            if(dropZoneLogic.plantedBeans[i] == null)
            {
                lastIndex = i;
                break;
            }
        }

        int pickupCardindex = 0;
        for(int i =0; i< handcardNum; i++)
        {
            if (HandCard[i] == cardYouhave)
            {
                pickupCardindex = i;
                break;
            }
        }

        HandCard[pickupCardindex].transform.position = myPlantingPos.transform.GetChild(farmIndex).position;
        dropZoneLogic.plantedBeans[lastIndex] = HandCard[pickupCardindex];
        HandCard[pickupCardindex] = null;
    }

    public void ClickCard(GameObject card)
    {
        cardData = card.GetComponent<CardData>();

        for (int i = 0; i < 5; i++)
        {
            bool isOverlap = false;

            for (int index =0; index <5; index++)
            {
                if(FindCardInHand(card) == mySuggest[index] && mySuggest[index] != -1)
                {
                    isOverlap = true;
                    break;
                }
            }

            if (!isOverlap)
            {
                //Cancel Trade.
                for(int index =0; index < 5; index++)
                {
                    if (mySuggest[index] == FindCardInHand(card))
                    {
                        Debug.Log("Cancel : " + index + "th Card");
                        HandCard[mySuggest[index]].transform.position += Vector3.down * 0.5f;
                        cardData.isTraded();

                        mySuggest[index] = -1;

                        cardYouhave = null;
                        return;
                    }
                }

                if(mySuggest[i] == -1)
                {
                    mySuggest[i] = FindCardInHand(card);
                    HandCard[mySuggest[i]].transform.position += Vector3.up * 0.5f;
                    cardData.isTraded();

                    cardYouhave = null;
                    return;
                }
            }
            else if(isOverlap)
            {
                //Cancel Trade.
                Debug.Log("Overlap");
                for (int index = 0; index < 5; index++)
                {
                    if (mySuggest[index] == FindCardInHand(card))
                    {
                        cardData = HandCard[mySuggest[index]].GetComponent<CardData>();
                        HandCard[mySuggest[index]].transform.position += Vector3.down * 0.5f;
                        cardData.isTraded();

                        mySuggest[index] = -1;

                        cardYouhave = null;
                        return;
                    }
                }

                if (mySuggest[i] == -1)
                {
                    Debug.Log("Trade : " + i + "th Card");
                    mySuggest[i] = FindCardInHand(card);
                    HandCard[mySuggest[i]].transform.position += Vector3.up * 0.5f;
                    cardData.isTraded();

                    cardYouhave = null;
                    return;
                }
                

                Debug.Log("Error");
            }
        }
    }
    int FindCardInHand(GameObject gameObj)
    {
        for(int i =0; i< handcardNum; i++)
        {
            if(gameObj == HandCard[i])
            {
                return i;
            }
        }

        return -1;
    }
    [PunRPC]
    void RPConSuggest(int[] suggestion, int index)
    {
        //TradeWindowLogic : CofirmSuggest
        //Arrange Suggesions.
        for(int i =0; i <5; i++)
        {
            if (suggestion[i] == -1)
            {
                for (int cindex = i; cindex < 5; cindex++)
                {
                    if (suggestion[cindex] != -1)
                    {
                        suggestion[i] = suggestion[cindex];
                        suggestion[cindex] = -1;
                        break;
                    }
                }
            }   
        }

        //Send mySuggestions
        mySuggest = suggestion;
        bellIndex = index;

        gameManagerLogic.getSuggest(bellIndex, myIndex, mySuggest);

        //Set Local Player UI
        int otherPlayerIndex = 0;
        for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i])
            {
                otherPlayerIndex = i;
                break;
            }
        }

        TradeWindowLogic tradeWindowLogic = gameManagerLogic.allUI[otherPlayerIndex].GetComponentInChildren<TradeWindowLogic>();
        tradeWindowLogic.setCard();
    }

    [PunRPC]
    void HandOverCard(int playerindex, int[] suggestionIndex)
    {
        //TradeWindowLogic : ConfirmSuggest

        //playerindex is index of who I receive cards from.
        //suggestionindex is other player's hand index.

        //Turn off Market Bean.
        for (int i = 0; i < 8; i++)
        {
            for (int UIindex = 0; UIindex < PhotonNetwork.PlayerList.Length; UIindex++)
            {
                gameManagerLogic.allUI[UIindex].transform.GetChild(5).GetChild(1 + bellIndex).GetChild(i).gameObject.SetActive(false);
            }
        }

        //Remove alters.
        for (int i = 0; i < 10; i++)
        {
            alterHandCards[i] = null;
        }

        PlayerLogic tradeOpponent = gameManagerLogic.playerLogics[playerindex].GetComponent<PlayerLogic>();
        SuggestedPlayerIndex = playerindex;
        tradeOpponent.isTrading = true;

        //Copy HandCard on alterHandCards.
        for (int i =0; i< 10; i++)
        {
            if (HandCard[i])
                alterHandCards[i] = HandCard[i];
        }
        for (int i = 0; i < 10; i++)
        {
            if (tradeOpponent.HandCard[i])
                tradeOpponent.alterHandCards[i] = tradeOpponent.HandCard[i];
        }

        //Remove All Card in Player's Hand.
        for (int i = 0; i < 10; i++)
        {
            HandCard[i] = null;
            tradeOpponent.HandCard[i] = null;
        }

        //Fill Hand with Suggested Cards.
        int num = 0;
        for (int i = 0; i < 5; i++)
        {
            if (suggestionIndex[i] != -1)
            {
                tradeOpponent.alterHandCards[suggestionIndex[i]].transform.position = cardPos[num].position;
                HandCard[num] = tradeOpponent.alterHandCards[suggestionIndex[i]];

                tradeOpponent.alterHandCards[suggestionIndex[i]] = null;
                num++;
            }
        }
        
        PV.RPC("SortingHand", RpcTarget.All, null);
        
        //Change 'CardData.playerLogic' to Local Player.
        for (int i = 0; i < 5; i++)
        {
            if (HandCard[i])
            {
                HandCard[i].GetComponent<CardData>().playerLogic = gameObject.GetComponent<PlayerLogic>();
            }
        }

        //Let Player plant Beans.
        canPlant = true;
        sugPlantingNum = 0; //Reset sugPlantingNum.

        //Send RPC to Opponent Player.
        if(gameManagerLogic.playerLogics[SuggestedPlayerIndex].GetComponent<PlayerLogic>().photonView.IsMine)
            gameManagerLogic.playerLogics[SuggestedPlayerIndex].GetComponent<PlayerLogic>().photonView.RPC("plantingMarketBean", RpcTarget.All, bellIndex);
    }

    [PunRPC]
    void plantingMarketBean(int bellindex)
    {
        //TradeButton : HandOverCard

        for (int i =0; i< handcardNum; i++)
        {
            if(alterHandCards[i])
                alterHandCards[i].SetActive(false);
        }

        PlayerLogic curTurnPL = gameManagerLogic.curTurnPlayer.GetComponent<PlayerLogic>();
        int suggNum = 0;

        //Fill Hand with Market Cards.
        for (int i =0; i< 5; i++)
        {
            if(gameManagerLogic.AllSuggestions[bellindex, curTurnPL.myIndex, i] != -1)
            {
                HandCard[i] = curTurnPL.alterHandCards[gameManagerLogic.AllSuggestions[bellindex, curTurnPL.myIndex, i]];
                curTurnPL.alterHandCards[gameManagerLogic.AllSuggestions[bellindex, curTurnPL.myIndex, i]] = null;

                HandCard[i].transform.position = cardPos[i].transform.position;
                suggNum++;
            }
        }


        HandCard[suggNum] = gameManagerLogic.marketBean[bellindex];


        HandCard[suggNum].transform.position = cardPos[suggNum].position;
        HandCard[suggNum].SetActive(true);

        SortingHand();

        myUI.transform.GetChild(11).GetComponent<TradeWindowLogic>().CloseWindow();

        //Change 'CardData.playerLogic' to Local Player.
        for (int i = 0; i < 5; i++)
        {
            if (HandCard[i])
            {
                HandCard[i].GetComponent<CardData>().playerLogic = gameObject.GetComponent<PlayerLogic>();
            }
        }

        canPlant = true;
        isTrading = true;

        //Reset GameManager.AllSuggestions
        gameManagerLogic.RemoveSuggestion(bellIndex);
    }

    void SellBeans()
    {
        if (!farmYouChoose)
            return;

        GameObject dropZones = myUI.transform.GetChild(5).GetChild(6).gameObject;
        DropZoneLogic FarmLogic = farmYouChoose.GetComponent<DropZoneLogic>();
        farmIndex = FarmLogic.farmIndex;
        bool canSelling = true;

        if (FarmLogic.beanIndex == -1)
        {
            Debug.Log("Null");
            return;
        }
        else
        {
            int num = didBuy ? 2 : 3;
            for (int i = 0; i < num; i++)
            {
                if (FarmLogic.beanNumb < dropZones.transform.GetChild(i).GetComponent<DropZoneLogic>().beanNumb && dropZones.transform.GetChild(i).GetComponent<DropZoneLogic>().beanNumb != 0)
                {
                    canSelling = false;
                    break;
                }
            }

            if (canSelling)
            {
                ActionButton.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);

                //Turn Off FarmCards.
                for (int index = 0; index < 3; index++)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        if (myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                            myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(false);
                        else
                            break;
                    }
                }
                //Turn Off handCards.
                for (int i = 0; i < 10; i++)
                {
                    if (HandCard[9 - i])
                        HandCard[9 - i].SetActive(false);
                }
            }
            else
            {
                Debug.Log("Cannot Sell.");
            }
        }
    }
    
    [PunRPC]
    void PlantBeans()
    {
        plantingNum = 0;
        canPlant = true;
    }
    public void SortingLayer()
    {
        int a = handcardNum - 1;
        for (int i = a; i >= 0; i--)
        {
            HandCard[i].SetActive(false);
            HandCard[i].SetActive(true);
        }
    }

    [PunRPC]
    public void GetSuggest(int[] suggestion, PlayerLogic tradeOpponent)
    {
        tradeOpponent.photonView.RPC("SortingHand", RpcTarget.All, null);
    }

    #endregion

    #region UI

    /* 
     * Children In MyUI *
       0. undo Button
       1. myScore
       2. NickName
       3. DeckNumber
       4. plantingScore
       5. BackGround
       6. left Bell 
       7. right Bell 
       8. Other Players' cameras
       9. Action Button
      10. StopWatch
      11. Trde Window
    */
    [PunRPC]
    public void SetMyUI()
    {
        //PlayerLogic : getMyIndex

        if(PV.IsMine)
            cameraManagerLogic.myplayerLogic = gameManagerLogic.playerLogics[myIndex].GetComponent<PlayerLogic>();

        // get MyUI
        myUI = gameManagerLogic.allUI[myIndex];

        // set MyUI
        //NickName
        myUI.transform.GetChild(2).GetComponentInChildren<Text>().text = PV.Owner.NickName;
        //CardPos
        for(int i =0; i < 10; i++)
        {
            cardPos[i] = myUI.transform.GetChild(5).GetChild(7).GetChild(i).transform;
        }
        //My Score
        myUI.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "0";
        //Action Button
        ActionButton = myUI.transform.GetChild(9).gameObject;
        //TradeWindow
        tradeWindow = myUI.transform.GetChild(11).gameObject;
        //Block
        myBlockTouch = gameManagerLogic.BlockTouch[myIndex];
        //Sorting Button
        myUI.transform.GetChild(15).GetComponent<Button>().onClick.AddListener(SortingHandRPC);
    }

    void SortingHandRPC()
    {
        gameManagerLogic.clickSound.Play();

        SortingHand();
    }

    [PunRPC]
    void getMyCamera()
    {
        //PlayerLogic : StartBtn
        cameraManagerLogic.getMyCamera();
    }
    [PunRPC]
    void SetScore()
    {
        //Call When Sell Beans.

        myUI.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = myScore.ToString();
    }

    public void CallSellRPC()
    {
        photonView.RPC("RPConSell", RpcTarget.All, farmIndex);
    }
    [PunRPC]
    void RPConSell(int farmindex)
    {
        //GameManager : ConfirmSell()
        DropZoneLogic FarmLogic = myUI.transform.GetChild(5).GetChild(6).GetChild(farmindex).GetComponent<DropZoneLogic>();

        //Add Score.
        myScore += FarmLogic.score;

        //Abandone Planted Beans.
        int num = 0;
        for (int i = 0; i < FarmLogic.beanNumb; i++)
        {
            while (gameManagerLogic.abandonedDeck[num])
            {
                num++;
            }

            FarmLogic.plantedBeans[i].SetActive(false);
            gameManagerLogic.abandonedDeck[num] = FarmLogic.plantedBeans[i];
            FarmLogic.plantedBeans[i] = null;
        }

        PV.RPC("SetScore", RpcTarget.All, null);

        //Reset beanIndex.
        FarmLogic.beanIndex = -1;
        FarmLogic.beanNumb = 0;
        FarmLogic.Scoring(-1);
    }
    public void CallSkipRPC()
    {
        if (PV.IsMine && isMyTurn)
            PV.RPC("skipLeftTime", RpcTarget.All, null);
    }
    [PunRPC]
    void skipLeftTime()
    {
        //PlayerLogic : DropCard

        gameManagerLogic.givenTime = 0;
    }
    public void CallBuyRPC()
    {
        photonView.RPC("RPConBuy", RpcTarget.All, null);
    }
    [PunRPC]
    void RPConBuy()
    {
        //GameManager : ConfirmBuy
        myUI.transform.GetChild(5).GetChild(6).GetChild(2).GetComponent<DropZoneLogic>().isBought = true;
        didBuy = true;

        myUI.transform.GetChild(5).GetChild(3).gameObject.SetActive(true);
        myUI.transform.GetChild(5).GetChild(4).gameObject.SetActive(false);

        ActionButton.transform.GetChild(4).gameObject.SetActive(false);
    }
    
    public IEnumerator ShowPanel(float time, string msg)
    {
        cameraManagerLogic.BackToMyCamera();

        //Reset Colors.
        Color TransparentWhite = new Color(1, 1, 1, 0);
        myBlockTouch.transform.GetChild(2).GetComponent<Image>().color = TransparentWhite;
        myBlockTouch.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = TransparentWhite;
        myBlockTouch.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>().color = TransparentWhite;

        WaitForSeconds waitForSec = new WaitForSeconds(0.1f);
        float full = time;

        Color newColor = new Color(1, 1, 1, 0);
        Color textColor = new Color(83f / 255f, 127f / 255f, 53f / 255f, 0);

        myBlockTouch.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>().text = msg;

        int a = (int)(full * 10);
        for (int i =0; i < a; i++)
        {
            newColor.a = 1f - time / full;
            textColor.a = 1f - time / full;

            myBlockTouch.transform.GetChild(2).GetComponent<Image>().color = newColor;
            myBlockTouch.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = newColor;
            myBlockTouch.transform.GetChild(2).GetChild(0).GetComponentInChildren<Text>().color = textColor;

            time -= full / (full * 10);

            yield return waitForSec;
        }

        yield return new WaitForSeconds(2f);
    }
    #endregion
}
