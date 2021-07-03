using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class CameraLogic : MonoBehaviour
{
    GameManager gameManagerLogic;
    public PlayerLogic myplayerLogic;

    public GameObject[] cameras;
    public GameObject[] CameraBtn;

    public int MyIndex;

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        SetCameraAddListener();
    }

    #region Camera Setting

    public void SetNickName()
    {
        int num = 0;
        for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            num = 0;
            for (int index = 0; index < PhotonNetwork.PlayerList.Length; index++)
            {
                if (i != index)
                {
                    CameraBtn[i].transform.GetChild(num).GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList[index].NickName;
                    num++;
                }
            }
            
            for(int index = PhotonNetwork.PlayerList.Length - 1; index < 3; index++)
            {
                CameraBtn[i].transform.GetChild(index).GetComponentInChildren<Text>().text = null;
                CameraBtn[i].transform.GetChild(index).GetComponent<Button>().onClick.AddListener(null);
            }
        }
    }

    void SetCameraAddListener()
    {
        if(MyIndex == 0)
        {
            CameraBtn[0].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GetSecondCamera);
            CameraBtn[0].transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GetThirdCamera);
            CameraBtn[0].transform.GetChild(2).GetComponent<Button>().onClick.AddListener(GetFourthCamera);
        }
        else if(MyIndex == 1)
        {
            CameraBtn[1].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GetFirstCamera);
            CameraBtn[1].transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GetThirdCamera);
            CameraBtn[1].transform.GetChild(2).GetComponent<Button>().onClick.AddListener(GetFourthCamera);
        }
        else if (MyIndex == 2)
        {
            CameraBtn[2].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GetFirstCamera);
            CameraBtn[2].transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GetSecondCamera);
            CameraBtn[2].transform.GetChild(2).GetComponent<Button>().onClick.AddListener(GetFourthCamera);
        }
        else if (MyIndex == 3)
        {
            CameraBtn[3].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(GetFirstCamera);
            CameraBtn[3].transform.GetChild(1).GetComponent<Button>().onClick.AddListener(GetSecondCamera);
            CameraBtn[3].transform.GetChild(2).GetComponent<Button>().onClick.AddListener(GetThirdCamera);
        }
    }

    #endregion

    #region Camera Button
    public void getMyCamera()
    {
        cameras[0].SetActive(false);
        
        cameras[MyIndex].SetActive(true);
        cameras[MyIndex].GetComponent<Camera>().depth = 5;

        gameManagerLogic.clickSound.Play();
        gameManagerLogic.BlockTouch[MyIndex].transform.GetChild(1).gameObject.SetActive(false); // Turn off Hand Bliner
    }

    public void BackToMyCamera()
    {
        for (int i = 0; i < 4; i++)
        {
            cameras[i].SetActive(false);
            cameras[i].GetComponent<Camera>().depth = 0;
        }

        cameras[MyIndex].SetActive(true);
        cameras[MyIndex].GetComponent<Camera>().depth = 5;

        gameManagerLogic.clickSound.Play();
    }

    public void GetFirstCamera()
    {
        for (int i = 0; i < 4; i++)
        {
            cameras[i].SetActive(false);
            cameras[i].GetComponent<Camera>().depth = 0;
        }


        cameras[0].SetActive(true);
        cameras[0].GetComponent<Camera>().depth = 5;

        gameManagerLogic.clickSound.Play();
    }

    public void GetSecondCamera()
    {
        for (int i = 0; i < 4; i++)
        {
            cameras[i].SetActive(false);
            cameras[i].GetComponent<Camera>().depth = 0;
        }


        cameras[1].SetActive(true);
        cameras[1].GetComponent<Camera>().depth = 5;

        gameManagerLogic.clickSound.Play();
    }
    public void GetThirdCamera()
    {
        for (int i = 0; i < 4; i++)
        {
            cameras[i].SetActive(false);
            cameras[i].GetComponent<Camera>().depth = 0;
        }


        cameras[2].SetActive(true);
        cameras[2].GetComponent<Camera>().depth = 5;

        gameManagerLogic.clickSound.Play();
    }
    public void GetFourthCamera()
    {
        for (int i = 0; i < 4; i++)
        {
            cameras[i].SetActive(false);
            cameras[i].GetComponent<Camera>().depth = 0;
        }


        cameras[3].SetActive(true);
        cameras[3].GetComponent<Camera>().depth = 5;

        gameManagerLogic.clickSound.Play();
    }

    #endregion

    #region playerButton
    public void SellButton()
    {
        //Turn Off Hand Cards.
        for (int i = 0; i < 10; i++)
        {
            if (myplayerLogic.HandCard[i])
                myplayerLogic.HandCard[i].SetActive(false);
        }

        Debug.Log(myplayerLogic.ActionButton.transform.GetChild(3).gameObject.name);

        myplayerLogic.ActionButton.transform.GetChild(3).gameObject.SetActive(true);
        myplayerLogic.canSell = true;

        gameManagerLogic.clickSound.Play();
    }
    public void ConfirmSell()
    {
        myplayerLogic.ActionButton.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        myplayerLogic.ActionButton.transform.GetChild(3).gameObject.SetActive(false);

        myplayerLogic.CallSellRPC();

        //Turn On FarmCards.
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
                else
                    break;
            }
        }
        //Turn On handCards.
        for (int i = 0; i < 10; i++)
        {
            if (myplayerLogic.HandCard[9 - i])
                myplayerLogic.HandCard[9 - i].SetActive(true);
        }

        gameManagerLogic.getPointSound.Play();

        myplayerLogic.ActionButton.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
        myplayerLogic.canSell = false;
    }
    public void CancelSell()
    {
        myplayerLogic.ActionButton.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        myplayerLogic.ActionButton.transform.GetChild(3).gameObject.SetActive(false);

        //Turn On handCards.
        for (int i = 0; i < 10; i++)
        {
            if (myplayerLogic.HandCard[9 - i])
                myplayerLogic.HandCard[9 - i].SetActive(true);
        }
        //Turn On FarmCards.
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
                else
                    break;
            }
        }

        gameManagerLogic.denySound.Play();

        myplayerLogic.ActionButton.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
        myplayerLogic.canSell = false;
    }


    public void skipButton()
    {
        if(myplayerLogic.plantingNum >= 1)
            myplayerLogic.CallSkipRPC();

        gameManagerLogic.clickSound.Play();
    }

    public void BuyFarmButton()
    {
        //Turn Off HandCard
        for (int i = 0; i < 10; i++)
        {
            if (myplayerLogic.HandCard[9 - i])
                myplayerLogic.HandCard[9 - i].SetActive(false);
        }
        //Turn On FarmCards.
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(false);
                else
                    break;
            }
        }

        gameManagerLogic.clickSound.Play();
        myplayerLogic.ActionButton.transform.GetChild(5).gameObject.SetActive(true);
    }
    public void ConfirmBuy()
    {
        if(myplayerLogic.myScore >= 3)
        {
            //Turn On HandCard
            for (int i = 0; i < 10; i++)
            {
                if (myplayerLogic.HandCard[9 - i])
                    myplayerLogic.HandCard[9 - i].SetActive(true);
            }
            //Turn On FarmCards.
            for (int index = 0; index < 3; index++)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                        myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
                    else
                        break;
                }
            }

            myplayerLogic.ActionButton.transform.GetChild(5).gameObject.SetActive(false);
            gameManagerLogic.confirmSound.Play();
            myplayerLogic.CallBuyRPC();
        }
        else
        {
            gameManagerLogic.denySound.Play();
        }
        
    }
    public void CancelBuy()
    {
        //Turn On HandCard
        for (int i = 0; i < 10; i++)
        {
            if (myplayerLogic.HandCard[9 - i])
                myplayerLogic.HandCard[9 - i].SetActive(true);
        }
        //Turn On FarmCards.
        for (int index = 0; index < 3; index++)
        {
            for (int i = 0; i < 15; i++)
            {
                if (myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i])
                    myplayerLogic.myUI.transform.GetChild(5).GetChild(6).GetChild(index).GetComponent<DropZoneLogic>().plantedBeans[i].SetActive(true);
                else
                    break;
            }
        }

        gameManagerLogic.clickSound.Play();

        myplayerLogic.ActionButton.transform.GetChild(5).gameObject.SetActive(false);
        myplayerLogic.ActionButton.transform.GetChild(3).gameObject.SetActive(false);
    }




    #endregion
}
