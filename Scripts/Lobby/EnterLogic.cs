using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class EnterLogic : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    string roomName;


    public void JoinInRoom()
    {
        if (roomName == null || roomName == "Empty")
        {
            Debug.Log("There is no Room.");
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName, null);
            PhotonNetwork.LoadLevel(2);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(gameObject.transform.childCount > 0)
        {
            roomName = gameObject.transform.GetChild(0).GetComponent<Text>().text.ToString();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("No room created");
    }
}
