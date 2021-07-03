using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

using Photon.Pun;
using Photon.Realtime;

public class StartLogic : MonoBehaviour
{
    private string gameVersion = "1";

    public GameObject dark;

    Image joiningLobby;

    void Start()
    {
        joiningLobby = dark.GetComponent<Image>();

        PhotonNetwork.GameVersion = gameVersion;
    }

    void Update()
    {
        
    }


    public void Connect()
    {
        if(PhotonNetwork.NickName == "")
        {
            Debug.Log("Please write your name");
            return;
        }
        PhotonNetwork.PhotonServerSettings.DevRegion = "kr";
        PhotonNetwork.ConnectUsingSettings();
        
        StartCoroutine(gettingDark());

        Invoke("jointoLobby", 2f);
    }

    IEnumerator gettingDark()
    {
        dark.SetActive(true);
        
        for (float i = 0; i <= 1; i += 0.05f)
        {
            joiningLobby.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void jointoLobby()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.LoadLevel(1);
    }
}
