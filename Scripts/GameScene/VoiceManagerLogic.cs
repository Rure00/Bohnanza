using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManagerLogic : Photon.Voice.Unity.VoiceConnection
{
    public void getID()
    {
        Settings.Server = "c15372d1 - 37e7 - 4926 - 8d5e-24c485b5ae34";

        ConnectUsingSettings();
    }
}
