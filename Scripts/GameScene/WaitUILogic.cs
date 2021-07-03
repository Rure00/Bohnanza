using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class WaitUILogic : MonoBehaviourPunCallbacks
{
    GameManager gameManager;

    public GameObject[] NameBox;
    public GameObject startBtn;
    public GameObject readyBtn;
    public GameObject cancelBtn;

    void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            NameBox[i].GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList[i].NickName;
        }
        for(int i = PhotonNetwork.PlayerList.Length; i < 4; i++)
        {
            NameBox[i].GetComponentInChildren<Text>().text = "Empty";
        }

    }
}
