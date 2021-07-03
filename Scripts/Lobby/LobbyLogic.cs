using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class LobbyLogic : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public GameObject[] roomsPanel;

    public GameObject makeRoomPanel;
    public GameObject roomNameInputfield;
    public GameObject blockTouch;

    Text testText;

    string roomName;

    byte maxPlayerPerRoom = 4;


    void Awake()
    {
        testText = roomNameInputfield.transform.GetChild(0).GetComponent<Text>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Debug.Log(PhotonNetwork.CountOfRooms);
    }

    #region Room Buttons

    public void CreateRoom()
    {
        makeRoomPanel.SetActive(true);
        testText.text = PhotonNetwork.NickName.ToString() + "'s Room";
    }

    public void CreateRoomConfirm()
    {
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = PhotonNetwork.NickName + "'s Room.";
            makeRoomPanel.SetActive(false);
        }
        else
        {
            makeRoomPanel.SetActive(false);
        }

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayerPerRoom, IsVisible = true });
        PhotonNetwork.LoadLevel(2);
    }
    public void CreateRoomCancel()
    {
        makeRoomPanel.SetActive(false);
    }

    public void SetRoomName(string value)
    {
        roomName = value;
    }

    public void quickEnter()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void refresh()
    {
        PhotonNetwork.JoinLobby();
    }

    #endregion

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Plaese try again...");
    }

    public override void OnCreatedRoom()
    {
        int roomsNum = PhotonNetwork.CountOfRooms;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        blockTouch.SetActive(true);
        int roomIndex = 0;

        foreach(RoomInfo roomInfo in roomList)
        {
            Text roomText = roomsPanel[roomIndex].transform.GetChild(0).GetComponent<Text>();
            Text NumbText = roomsPanel[roomIndex].transform.GetChild(1).GetComponent<Text>();

            if(roomInfo == null)
            {
                roomText.text = "Empty";
                NumbText.text = "( 0 / 0 )";
            }
            else
            {
                roomText.text = roomInfo.Name;
                NumbText.text = "( " + roomInfo.PlayerCount + " / 4 )";
            }
            
            roomIndex++;
        }

        blockTouch.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("You Joined in " + PhotonNetwork.CurrentRoom.Name);
    }
}
