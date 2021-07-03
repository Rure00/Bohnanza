using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

public class RecorderLogic : MonoBehaviourPun
{
    GameManager gameManagerLogic;

    public KeyCode pushButton = KeyCode.P;
    public KeyCode activeButton = KeyCode.M;

    public Recorder voiceRecorder;
    private PhotonView view;

    bool isConnected;

    void Start()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        view = photonView;
        voiceRecorder.TransmitEnabled = false;

        voiceRecorder.IsRecording = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(pushButton) && !isConnected)
        {
            if(view.IsMine)
            {
                voiceRecorder.TransmitEnabled = true;

                gameManagerLogic.clickSound.Play();
            }
        }
        else if(Input.GetKeyUp(pushButton) && !isConnected)
        {
            if(view.IsMine)
            {
                voiceRecorder.TransmitEnabled = false;

                gameManagerLogic.denySound.Play();
            }
        }

        if(Input.GetKeyDown(activeButton) && !isConnected)
        {
            if (view.IsMine)
            {
                voiceRecorder.TransmitEnabled = true;
                isConnected = true;

                gameManagerLogic.clickSound.Play();
            } 
        }
        else if(Input.GetKeyDown(activeButton) && isConnected)
        {
            if (view.IsMine)
            {
                voiceRecorder.TransmitEnabled = false;
                isConnected = false;

                gameManagerLogic.denySound.Play();
            }
        }
    }
}
