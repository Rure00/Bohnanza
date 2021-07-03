using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class ResultUILogic : MonoBehaviourPunCallbacks
{
    public GameObject[] InfoBoxGroup;
    GameManager gameManagerLogic;

    PlayerLogic[] Rank;

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            Rank[i] = gameManagerLogic.playerLogics[i].GetComponent<PlayerLogic>();
        }
    }

    public void ShowResult()
    {
        for(int i =0; i< PhotonNetwork.PlayerList.Length; i++)
        {
            InfoBoxGroup[i].SetActive(true);
        }
    }
    void getRanking()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            for (int index = i; index < PhotonNetwork.PlayerList.Length; index++)
            {
                if(Rank[i].myScore < Rank[index].myScore)
                {
                    PlayerLogic alter = Rank[i];
                    Rank[i] = Rank[index];
                    Rank[index] = alter;
                }
            }
        }
    }
    void setNickName()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            InfoBoxGroup[i].transform.GetChild(1).GetComponentInChildren<Text>().text = Rank[i].gameObject.name;
        }
    }
    void setScore()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            InfoBoxGroup[i].transform.GetChild(0).GetComponentInChildren<Text>().text = "Score is " + Rank[i].myScore.ToString();
        }
    }

    public void ExitGame()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
}
